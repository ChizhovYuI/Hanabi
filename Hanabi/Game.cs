using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public static class Game
    {
        public static List<PlayerCard>[] PlayersDecks { get; private set; }
        public static Queue<Card> Deck { get; private set; }
        public static int Turn { get; private set; }
        public static int Score { get; private set; }
        public static int Risk { get; private set; }

        public static string ProcessCommand(string commandWithArgs)
        {
            UpdateGame(commandWithArgs);
            return _isFinished ? string.Format("Turn: {0}, cards: {1}, with risk: {2}", Turn, Score, Risk) : null;
        }

        private static void UpdateGame(string commandWithArgs)
        {
            string command = CommandsStartsWith.Single(commandWithArgs.StartsWith);
            string[] args = commandWithArgs.Remove(0, command.Length).Split();
            Turn++;
            if (command.Equals(StartGameStartsWith))
            {
                StartNewGame(args);
                return;
            }
            _isFinished = command.Equals(TellColorStartsWith) || command.Equals(TellRankStartsWith) ? TellColorOrRank(command, args) : PlayOrDropCard(command, args);
        }

        private static void StartNewGame(string[] cards)
        {
            Turn = 0;
            Score = 0;
            Risk = 0;
            _isFinished = false;
            Table = new Dictionary<Color, int>();
            foreach (Color color in Enum.GetValues(typeof(Color)).Cast<Color>())
                Table.Add(color, 0);
            PlayersDecks = new List<PlayerCard>[PlayersCount];
            for (int i = 0; i < PlayersCount; i++)
                PlayersDecks[i] = GetPlayerDeck(cards, i);
            Deck = CreateDeck(cards);
        }

        private static List<PlayerCard> GetPlayerDeck(string[] cards, int playerNumber)
        {
            return cards.Skip(PlayerCardsCount * playerNumber).Take(PlayerCardsCount).Select(e => new PlayerCard(e)).ToList();
        }

        private static Queue<Card> CreateDeck(string[] cards)
        {
            Queue<Card> deck = new Queue<Card>();
            cards.Skip(PlayerCardsCount * PlayersCount).ToList().ForEach(card => deck.Enqueue(new Card(card)));
            return deck;
        }

        private static bool TellColorOrRank(string command, string[] colorOrRankAndCards)
        {
            List<PlayerCard> playerDeck = GetCurrentPlayerDeckForTell();
            List<PlayerCard> selectedCards = colorOrRankAndCards.Skip(3).Select(e => playerDeck[e.ParseToInt()]).ToList();
            string value = colorOrRankAndCards[0];
            List<PlayerCard> expectedCards = command.Equals(TellColorStartsWith) ? TellColor(playerDeck, value) : TellRank(playerDeck, value);
            return !Enumerable.SequenceEqual(selectedCards, expectedCards);
        }

        private static List<PlayerCard> TellColor(List<PlayerCard> playerDeck, string value)
        {
            Color selectedColor = value.ParseToColor();
            playerDeck.ForEach(card => card.AddColorInfo(selectedColor));
            return playerDeck.FindAll(e => e.Color == selectedColor);
        }

        private static List<PlayerCard> TellRank(List<PlayerCard> playerDeck, string value)
        {
            int selectedRank = value.ParseToInt();
            playerDeck.ForEach(card => card.AddRankInfo(selectedRank));
            return playerDeck.FindAll(e => e.Rank == selectedRank);
        }

        private static bool PlayOrDropCard(string command, string[] card)
        {
            List<PlayerCard> playerDeck = GetCurrentPlayerDeckForPlayOrDrop();
            int cardNumber = card.Single().ParseToInt();
            PlayerCard selectedCard = playerDeck[cardNumber];
            playerDeck.RemoveAt(cardNumber);
            if (command.Equals(PlayCardStartsWith) && PlayCard(selectedCard))
                return true;
            playerDeck.Add(new PlayerCard(Deck.Dequeue()));
            return CheckFinishedGame();
        }

        private static bool PlayCard(PlayerCard card)
        {
            if (!CheckCorrectPlayCard(card))
                return true;
            Table[card.Color]++;
            Score++;
            if (CheckRiskPlayCard(card))
                Risk++;
            return false;
        }

        private static readonly Func<List<PlayerCard>> GetCurrentPlayerDeckForPlayOrDrop = () =>
            PlayersDecks[(Turn - 1) % PlayersCount];

        private static readonly Func<List<PlayerCard>> GetCurrentPlayerDeckForTell = () =>
            PlayersDecks[Turn % PlayersCount];

        private static readonly Func<PlayerCard, bool> CheckCorrectPlayCard = card =>
            card.Rank == Table[card.Color] + 1;

        private static readonly Func<PlayerCard, bool> CheckRiskPlayCard = (card) =>
            !((card.IsKnowRank || card.NotThisRanks.All(e => e.Value)) &&
              (card.IsKnowColor || Table.All(e => e.Value == 0) || card.NotThisColors.All(e => e.Value) ||
               card.NotThisColors.Where(e => !e.Value).All(e => Table[e.Key] == card.Rank - 1)));

        private static readonly Func<bool> CheckFinishedGame = () =>
            Deck.Count == 0 || Table.All(pair => pair.Value == Card.MaxRank);

        private static readonly List<string> CommandsStartsWith = new List<string>()
        {
            StartGameStartsWith,
            TellColorStartsWith,
            TellRankStartsWith,
            PlayCardStartsWith,
            DropCardStartsWith
        };

        private static bool _isFinished;
        private static Dictionary<Color, int> Table;

        private const int PlayersCount = 2;
        private const int PlayerCardsCount = 5;

        private const string StartGameStartsWith = "Start new game with deck ";
        private const string TellColorStartsWith = "Tell color ";
        private const string TellRankStartsWith = "Tell rank ";
        private const string PlayCardStartsWith = "Play card ";
        private const string DropCardStartsWith = "Drop card ";
    }
}