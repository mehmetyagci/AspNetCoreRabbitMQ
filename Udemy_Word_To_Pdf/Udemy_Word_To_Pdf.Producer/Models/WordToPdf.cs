﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Udemy_Word_To_Pdf.Producer.Models
{
    public class WordToPdf
    {
        public string Email { get; set; }

        public IFormFile WordFile { get; set; }
        
    }
}
