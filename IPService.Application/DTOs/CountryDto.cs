using System;
using System.Collections.Generic;
using System.Text;

namespace IPService.Application.DTOs
{
   
    
    public record class CountryDto(string TwoLetterCode, string ThreeLetterCode, string CountryName);
    
}
