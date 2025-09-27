using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teoryAvtom1
{
    // Класс, описывающий Деталь
    public class Detail
    {
        public DetailType Type { get; set; }
        public DetailColor Color { get; set; }

        public Detail(DetailType type, DetailColor color)
        {
            Type = type;
            Color = color;
        }

        // Для удобства вывода в текст
        public override string ToString()
        {
            return $"{Color} {Type}";
        }
    }
}