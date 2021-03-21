using System;
using System.Collections.Generic;

namespace TextRecognition.DbModel
{
    public class TextModel
    {
        public Guid Id { get; set; }
        public List<string> Lines { get; set; }
    }
}