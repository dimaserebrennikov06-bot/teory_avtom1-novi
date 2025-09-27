using System;
using System.Collections.Generic;
using System.Linq;

namespace teoryAvtom1
{
    public class GameState
    {
        public List<Box> Boxes { get; set; } = new List<Box>();
        public Detail CurrentDetail { get; set; } = null;
        public int CurrentDetailPosition { get; set; } = 0;
        public bool IsRunning { get; set; } = false;

        public List<DetailType> AllowedTypes { get; set; } = new List<DetailType>();
        public List<DetailColor> AllowedColors { get; set; } = new List<DetailColor>();

        private Random random = new Random();

        public const int ConveyorLength = 100;
        public const int DropZonePosition = 80;

        public GameState()
        {
            AllowedTypes = new List<DetailType>();
            AllowedColors = new List<DetailColor>();
        }

        public void CreateBoxes()
        {
            Boxes.Clear();
            int[] counts = { 3, 5, 1, 7, 6, 2, 4, 4 };
            int index = 0;

            foreach (var type in AllowedTypes)
            {
                foreach (var color in AllowedColors)
                {
                    if (index < counts.Length)
                    {
                        Boxes.Add(new Box(type, color, counts[index]));
                        index++;
                    }
                }
            }
        }

        public void GenerateSingleDetail()
        {
            // Получаем ВСЕ активные комбинации (тип + цвет) из НЕЗАПОЛНЕННЫХ ящиков
            var availableCombinations = Boxes
                .Where(b => b.IsActive && !b.IsFull)
                .Select(b => new { Type = b.TargetType, Color = b.TargetColor })
                .Distinct()
                .ToList();

            // Если нет доступных комбинаций - игра окончена
            if (availableCombinations.Count == 0)
            {
                CurrentDetail = null;
                IsRunning = false;
                return;
            }

            // Выбираем случайную комбинацию из доступных
            var randomCombination = availableCombinations[random.Next(availableCombinations.Count)];

            CurrentDetail = new Detail(randomCombination.Type, randomCombination.Color);
            CurrentDetailPosition = 0;
        }

        public bool ProcessCurrentDetail()
        {
            if (CurrentDetail == null)
                return false;

            foreach (var box in Boxes)
            {
                if (box.TryAcceptDetail(CurrentDetail))
                {
                    CurrentDetail = null;
                    return true;
                }
            }
            return false;
        }

        public void MoveConveyor()
        {
            if (CurrentDetail == null)
            {
                GenerateSingleDetail(); // Новый вызов без параметров
                return;
            }

            CurrentDetailPosition += 5;

            if (CurrentDetailPosition >= DropZonePosition)
            {
                if (ProcessCurrentDetail())
                {
                    GenerateSingleDetail(); // Новый вызов без параметров
                }
            }
        }

        public void ResetGame()
        {
            Boxes.Clear();
            CurrentDetail = null;
            CurrentDetailPosition = 0;
            IsRunning = false;
        }
    }
}