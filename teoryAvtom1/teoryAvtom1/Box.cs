using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teoryAvtom1
{
    // Класс, описывающий Ящик
    public class Box
    {
        public DetailType TargetType { get; set; }
        public DetailColor TargetColor { get; set; }
        public int CurrentCount { get; set; }
        public int MaxCount { get; set; } = 8;
        public bool IsActive { get; set; } = true;
        public string DisplayName { get; private set; }

        public bool IsFull => CurrentCount >= MaxCount;

        public Box(DetailType type, DetailColor color, int initialCount)
        {
            TargetType = type;
            TargetColor = color;
            CurrentCount = initialCount;
            DisplayName = $"{color} {type}";
        }

        // Метод, чтобы попытаться "положить" деталь в этот ящик
        public bool TryAcceptDetail(Detail detail)
        {
            if (IsActive && !IsFull && detail.Type == TargetType && detail.Color == TargetColor)
            {
                CurrentCount++;

                if (IsFull)
                {
                    IsActive = false;
                }
                return true;
            }
            return false;
        }
    }
}