using System.Collections.Generic;

namespace GGXrdReversalTool.Contributors;


public class Contributor
{
    public string Name { get; set; }
    public Link Link { get; set; }
    public string Role { get; set; }



    public static IEnumerable<Contributor> AppContributors => new[]
    {
        new Contributor { Name = "//FIXMESTEVE", Role = "Technical advice", Link = new Link { Name = "@TomSolacroup", Url = "https://twitter.com/TomSolacroup"}},
        new Contributor { Name = "Labreezy", Role = "Technical advice", Link = new Link { Name = "Labryz", Url = "https://www.youtube.com/watch?v=a3MRA2lIRds"}},
        new Contributor { Name = "PC_volt", Role = "Test", Link = new Link { Name = "@PC_volt", Url = "https://twitter.com/PC_volt"}},
        new Contributor { Name = "Rygz", Role = "Test", Link = new Link { Name = "@Rygzz", Url = "https://twitter.com/Rygzz"}},
        new Contributor { Name = "Eskagasse", Role = "Test", Link = new Link { Name = "@Eskagasse", Url = "https://twitter.com/Eskagasse"}},
        new Contributor { Name = "Pangaea", Role = "Technical advice", Link = new Link { Name = "@Pangaea__", Url = "https://twitter.com/Pangaea__"}},
        new Contributor { Name = "Fenom", Role = "Test"},
        new Contributor { Name = "CFG", Role = "Technical advice", Link = new Link { Name = "@CaptainFlameGuy", Url = "https://twitter.com/CaptainFlameGuy"}},
        new Contributor { Name = "Masker", Role = "Test", Link = new Link { Name = "@Masker_Sol", Url = "https://twitter.com/Masker_Sol"}},
        new Contributor { Name = "Wandrall", Role = "Test"},
        new Contributor { Name = "Blubo", Role = "Test", Link = new Link { Name = "@Blubolouis", Url = "https://twitter.com/Blubolouis"}},
        new Contributor { Name = "Don Pietro", Role = "Test"},
        new Contributor { Name = "AshSux", Role = "App Icon", Link = new Link { Name = "@Ash_Sux", Url = "https://twitter.com/Ash_Sux"}},
        new Contributor { Name = "Worsety", Role = "Dev contribution"},
        new Contributor { Name = "Mirri", Role = "Dev contribution"},
        new Contributor { Name = "kkots", Role = "Technical advice"},
         
    };
}