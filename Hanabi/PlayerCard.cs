using System;
using System.Collections.Generic;

namespace Hanabi
{
    public class PlayerCard : Card
    {
        public PlayerCard(string card)
            : base(card)
        {
            Initialization();
        }

        public PlayerCard(Card card)
        {
            Color = card.Color;
            Rank = card.Rank;
            Initialization();
        }

        public bool IsKnowColor { get; private set; }
        public bool IsKnowRank { get; private set; }
        public IDictionary<Color, bool> NotThisColors { get; private set; }
        public IDictionary<int, bool> NotThisRanks { get; private set; }

        public void AddColorInfo(Color selectedColor)
        {
            if (Color == selectedColor)
            {
                IsKnowColor = true;
                return;
            }
            NotThisColors[selectedColor] = true;
        }

        public void AddRankInfo(int selectedRank)
        {
            if (Rank == selectedRank)
            {
                IsKnowRank = true;
                return;
            }
            NotThisRanks[selectedRank] = true;
        }

        public override string ToString()
        {
            return string.Concat(Color, Rank, "  Know color: ", IsKnowColor, " Know rank: ", IsKnowRank);
        }

        private void Initialization()
        {
            IsKnowColor = false;
            IsKnowRank = false;
            InitializationNotThisColors();
            InitializationNotThisRanks();
        }

        private void InitializationNotThisColors()
        {
            NotThisColors = new Dictionary<Color, bool>();
            foreach (Color color in Enum.GetValues(typeof(Color)))
                if (Color != color)
                    NotThisColors.Add(color, false);
        }

        private void InitializationNotThisRanks()
        {
            NotThisRanks = new Dictionary<int, bool>();
            for (int i = MinRank; i <= MaxRank; i++)
                if (Rank != i)
                    NotThisRanks.Add(i, false);
        }
    }
}