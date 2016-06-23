using System;
using System.Collections.Generic;

namespace Hanabi
{
    public class Card
    {
        public Color Color { get; protected set; }

        public const int MinRank = 1;
        public const int MaxRank = 5;

        public int Rank
        {
            get
            {
                return _rank;
            }
            protected set
            {
                if (value < MinRank || value > MaxRank)
                    throw new ArgumentOutOfRangeException();
                _rank = value;
            }
        }

        public Card(string card)
        {
            if (!Colors.ContainsKey(card[0]) || !char.IsDigit(card[1]))
                throw new ArgumentException();
            Color = Colors[card[0]];
            Rank = int.Parse(card[1].ToString());
        }

        public override string ToString()
        {
            return string.Concat(Color.ToString(), " ", Rank);
        }

        public override bool Equals(object obj)
        {
            Card card = (Card)obj;
            return Color == card.Color && Rank == card.Rank;
        }

        public override int GetHashCode()
        {
            return Rank + (int)Color * 5;
        }

        protected Card() { }

        private static readonly Dictionary<char, Color> Colors = new Dictionary<char, Color>
        {
            {'R', Color.Red},
            {'G', Color.Green},
            {'B', Color.Blue},
            {'Y', Color.Yellow},
            {'W', Color.White}
        };

        private int _rank;
    }
}