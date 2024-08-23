using GGXrdReversalTool.Library.Domain.Characters;

namespace GGXrdReversalTool.Library.Presets;

public class Preset
{
    public Character? Character { get; set; }

    public IEnumerable<CharacterMove> CharacterMoves { get; private init; } = Enumerable.Empty<CharacterMove>();

    public static IEnumerable<Preset> Presets => new[]
    {
        new Preset
        {
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Blitz High", Input = "!5SH" },
                new CharacterMove { Name = "Blitz Low", Input = "!2SH" },
                new CharacterMove { Name = "Backdash", Input = "4,5,!4" },
                new CharacterMove { Name = "Backdash into standing FD", Input = "4,5,!4,5*10,4PK*200"},
                new CharacterMove { Name = "Backdash into crouching FD", Input = "4,5,!4,5*10,1PK*200"},
                new CharacterMove { Name = "Backdash YRC", Input = "yrc 4,5,!4,5*5,5PKS"},
                new CharacterMove { Name = "FD Jump", Input = "!7,1PK*200"},
                new CharacterMove { Name = "FD Superjump", Input = "!2,7,1PK*200"},
                new CharacterMove { Name = "Gold Burst", Input = "!5DH"},
                new CharacterMove { Name = "Throw + 6P OS", Input = "!6PH*3"},
                new CharacterMove { Name = "Throw + cS OS", Input = "!6SH*3"}

            }
        },
        new Preset
        {
            Character = Character.Sol,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Gun Flame", Input = "2,3,!6P" },
                new CharacterMove { Name = "Gun Flame (Feint)", Input = "2,1,!4P" },
                new CharacterMove { Name = "Volcanic Viper H (DP)", Input = "6,2,!3H" },
                new CharacterMove { Name = "Volcanic Viper S (DP)", Input = "6,2,!3S" },
                new CharacterMove { Name = "Tyrant Rave", Input = "6,3,2,1,4,!6H"},
                new CharacterMove { Name = "Burst Tyrant Rave", Input = "6,3,2,1,4,!6D"},
            }
        },
        new Preset
        {
            Character = Character.Ky,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Stun Edge", Input = "2,3,!6S" },
                new CharacterMove { Name = "Charged Stun Edge", Input = "2,3,!6H" },
                new CharacterMove { Name = "Vapor Thrust H (DP)", Input = "6,2,!3H" },
                new CharacterMove { Name = "Vapor Thrust S (DP)", Input = "6,2,!3S" },
                new CharacterMove { Name = "Ride The Lightning", Input = "6,3,2,1,4,!6H"},
                new CharacterMove { Name = "Burst Ride The Lightning", Input = "6,3,2,1,4,!6D"},
            }
        },
        new Preset
        {
            Character = Character.May,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Ultimate Spinning Whirlwind", Input = "6,3,2,1,!4H" },
                new CharacterMove { Name = "Great Yamada Attack", Input = "2,3,6,2,3,!6S" },
                new CharacterMove { Name = "Burst Great Yamada Attack", Input = "2,3,6,2,3,!6D" }
            }
        },
        new Preset
        {
            Character = Character.Millia,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Winger", Input = "2,1,4,1,2,3,!6H" },
                new CharacterMove { Name = "Burst Winger", Input = "2,1,4,1,2,3,!6D" }
            }
        },
        new Preset
        {
            Character = Character.Zato,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Amorphous", Input = "6,3,2,1,4,!6H" },
                new CharacterMove { Name = "Burst Amorphous", Input = "6,3,2,1,4,!6D" }
            }
        },
        new Preset
        {
            Character = Character.Potemkin,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Giganter Kai", Input = "6,3,2,1,4,!6H" },
                new CharacterMove { Name = "Giganter Bullet Kai", Input = "6,3,2,1,4,!6H,5*42,4,1,2,3,6,4,1,2,3,6P" },
                new CharacterMove { Name = "Heavenly Potemkin Buster", Input = "2,3,6,2,3,!6S" },
                new CharacterMove { Name = "Burst Heavenly Potemkin Buster", Input = "2,3,6,2,3,!6S" },
                new CharacterMove { Name = "Backdash Buster", Input = "4,5,!4,5*13,6,3,2,1,4,6P" }
            }
        },
        new Preset
        {
            Character = Character.Chipp,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Beta Blade (DP)", Input = "6,2,!3S" },
                new CharacterMove { Name = "Zansei Rouga", Input = "6,3,2,1,4,!6H" },
                new CharacterMove { Name = "Burst Zansei Rouga", Input = "6,3,2,1,4,!6D" },
                new CharacterMove { Name = "Banki Mesai", Input = "2,3,6,2,3,!6K" }
            }
        },
        new Preset
        {
            Character = Character.Faust,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Stimulating Fists of Annihilation", Input = "2,3,6,2,3,!6S" },
                new CharacterMove { Name = "Burst Stimulating Fists of Annihilation", Input = "2,3,6,2,3,!6D" },
                new CharacterMove { Name = "5D", Input = "!5D" }
            }
        },
        new Preset
        {
            Character = Character.Axl,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Artemis Hunter AKA Benten (DP)", Input = "6,2,!3S" },
                new CharacterMove { Name = "Sickle Storm", Input = "2,3,6,3,2,1,!4H" },
                new CharacterMove { Name = "Burst Sickle Storm", Input = "2,3,6,3,2,1,!4D" }
            }
        },
        new Preset
        {
            Character = Character.Venom,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Burst Dark Angel", Input = "2,3,6,3,2,1,!4D" }
            }
        },
        new Preset
        {
            Character = Character.Slayer,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "BDC K Mappa Hunch", Input = "4,5,!4*4,2,3,6,9K" },
                new CharacterMove { Name = "BDC P Mappa Hunch", Input = "4,5,!4*4,2,3,6,9K" },
                new CharacterMove { Name = "BDC Bloodsucking Universe (Grab)", Input = "6,3,2,1,4,5,!4*4,7H" },
                new CharacterMove { Name = "BDC Dead on Time", Input = "6,3,2,1,4,5,!4*4,6,9S" },
                new CharacterMove { Name = "BDC Burst Dead on Time", Input = "6,3,2,1,4,5,!4*4,6,9D" },
                new CharacterMove { Name = "Burst Dead on Time", Input = "6,3,2,1,4,!6D" },
                new CharacterMove { Name = "BDC P Dandy Step -> Pilebunker", Input = "2,1,4,5,!4*4,7P,5*15,5P" },
                new CharacterMove { Name = "Backdash Jump FD", Input = "4,5,!4*5,7,1PK*50" },
                new CharacterMove { Name = "Eternal Wings", Input = "2,3,6,2,3,!6H" }
            }
        },
        new Preset
        {
            Character = Character.Ino,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Longing Desperation", Input = "6,3,2,1,4,!6H" },
                new CharacterMove { Name = "Burst Longing Desperation", Input = "6,3,2,1,4,!6D" },
                new CharacterMove { Name = "Vertical Chemical Love YRC", Input = "2,1,!4S,5*5,5PKS" }
            }
        },
        new Preset
        {
            Character = Character.Bedman,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Sinusoidal Helios", Input = "6,3,2,1,4,!6H" },
                new CharacterMove { Name = "Burst Sinusoidal Helios", Input = "6,3,2,1,4,!6D" }
            }
        },
        new Preset
        {
            Character = Character.Ramlethal,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Burst Calvados (Need Swords Equipped)", Input = "6,3,2,1,4,!6D" },
                new CharacterMove { Name = "Explode", Input = "2,3,6,3,2,1,!4K" },
                new CharacterMove { Name = "Sildo Detruo", Input = "2,1,!4K" }
            }
        },
        new Preset
        {
            Character = Character.Sin,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Hawk Baker (DP)", Input = "6,2,!3S" },
                new CharacterMove { Name = "Hawk Baker into Elk Hunt", Input = "6,2,!3S,5*5,2,3,6K" },
                new CharacterMove { Name = "R.T.L", Input = "6,3,2,1,4,!6H" },
                new CharacterMove { Name = "Burst R.T.L", Input = "6,3,2,1,4,!6D" }
            }
        },
        new Preset
        {
            Character = Character.Elphelt,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Judge Better Half", Input = "2,3,6,2,3,!6S" },
                new CharacterMove { Name = "Burst Judge Better Half", Input = "2,3,6,2,3,!6D" }
            }
        },
        new Preset
        {
            Character = Character.Leo,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Eisen Sturm H (Flashkick) (SET DUMMY TO CROUCHING OR IT WONT WORK)", Input = "!8H" },
                new CharacterMove { Name = "Eisen Sturm S (Flashkick) (SET DUMMY TO CROUCHING OR IT WONT WORK)", Input = "!8S" },
                new CharacterMove { Name = "Leidenschaft Dirigent", Input = "6,3,2,1,4,!6H" }
            }
        },
        new Preset
        {
            Character = Character.Johnny,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "That's My Name (J)", Input = "2,3,6,3,2,1,!4H" },
                new CharacterMove { Name = "Burst That's My Name (J)", Input = "2,3,6,3,2,1,!4D" }
            }
        },
        new Preset
        {
            Character = Character.JackO,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "2D (DP)", Input = "!2D" },
                new CharacterMove { Name = "Forever Elysion Driver", Input = "4,1,2,3,6,9,8,!7P" },
                new CharacterMove { Name = "Burst Calvados", Input = "2,1,!4D" }
            }
        },
        new Preset
        {
            Character = Character.Jam,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Parry -> P", Input = "4,5,!6,5P" },
                new CharacterMove { Name = "Parry -> PP", Input = "4,5,!6,5P,5*31,5P,5,5P,5,5P,5,5P" },
                new CharacterMove { Name = "Parry -> PK", Input = "4,5,!6,5P,5*31,5K,5,5K,5,5K,5,5K" },
                new CharacterMove { Name = "Parry FD Cancel on Whiff", Input = "4,5,!6,5*4,1PK*200" },
                new CharacterMove { Name = "Kenroukaku (DP)", Input = "6,2,!3K" },
                new CharacterMove { Name = "Kenroukaku (DP) into Gekirin", Input = "6,2,!3K,5*10,2,1,4K" },
                new CharacterMove { Name = "Bao Saishinshou", Input = "2,3,6,2,3,!6H" },
                new CharacterMove { Name = "Choukyaku Hou'Oushou", Input = "6,3,2,1,4,!6S" },
                new CharacterMove { Name = "Burst Choukyaku Hou'Oushou", Input = "6,3,2,1,4,!6D" }
            }
        },
        new Preset
        {
            Character = Character.Haehyun,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Blue Tuning Ball", Input = "2,3,6S" },
                new CharacterMove { Name = "Red Tuning Ball", Input = "2,3,6H" },
                new CharacterMove { Name = "Four Tigers Sword", Input = "6,2,3K" },
                new CharacterMove { Name = "Four Tigers Sword [Max]", Input = "6,2,3K,5K*56" },
                new CharacterMove { Name = "Four Tigers Sword (Reverse)", Input = "6,2,3K,4K*6" },
                new CharacterMove { Name = "Four Tigers Sword (Reverse) [Max]", Input = "6,2,3K,4K*19" },
                new CharacterMove { Name = "Blue Shi-Shiinken", Input = "6,2,3k*10,5*7,4k*12" },
                new CharacterMove { Name = "Red Shi-Shiinken", Input = "6,2,3k*10,5*7,4k*25" },
                new CharacterMove { Name = "Falcon Dive", Input = "2,1,4K" },
                new CharacterMove { Name = "Falcon Dive [Max]", Input = "2,1,4K" },
                new CharacterMove { Name = "Falcon Dive (Reverse)", Input = "2,1,4K*7" },
                new CharacterMove { Name = "Falcon Dive (Reverse) [Max]", Input = "2,1,4K*7,5k*59" },
                new CharacterMove { Name = "Air Falcon Dive", Input = "2,1,4,7*5,7k" },
                new CharacterMove { Name = "Enlightened 3000 Palm Strike", Input = "2,3,6,2,3,!6H" },
                new CharacterMove { Name = "Burst Enlightened 3000 Palm Strike", Input = "2,3,6,2,3,!6D" },
                new CharacterMove { Name = "Enlightened 3000 Palm Strike [Max]", Input = "2,3,6,2,3,6H,5*80,5H*195" },
                new CharacterMove { Name = "Celestial Tuning Ball", Input = "2,3,6,2,3,6S" }
            }
        },
        new Preset
        {
            Character = Character.Raven,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Getreuer", Input = "2,1,4,2,1,!4H" },
                new CharacterMove { Name = "Burst Getreuer", Input = "2,1,4,2,1,!4D" },
                new CharacterMove { Name = "Stance", Input = "2,1,!4K*50" }
            }
        },
        new Preset
        {
            Character = Character.Dizzy,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Don't be overprotective (Mirror)", Input = "6,3,2,1,4,!6P" },
                new CharacterMove { Name = "Burst Imperial Ray", Input = "6,3,2,1,4,!6D" }
            }
        },
        new Preset
        {
            Character = Character.Baiken,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Azami -> Kuchinashi", Input = "!5HS,5P" },
                new CharacterMove { Name = "Azami -> Mawarikomi", Input = "!5HS,5K" },
                new CharacterMove { Name = "Azami -> Sakura", Input = "!5HS,5S" },
                new CharacterMove { Name = "Azami -> Rokkonsogi", Input = "!5HS,5H" },
                new CharacterMove { Name = "Low Azami -> Kuchinashi", Input = "!2HS,5P" },
                new CharacterMove { Name = "Low Azami -> Mawarikomi", Input = "!2HS,5K" },
                new CharacterMove { Name = "Low Azami -> Sakura", Input = "!2HS,5S" },
                new CharacterMove { Name = "Low Azami -> Rokkonsogi", Input = "!2HS,5H" },
                new CharacterMove { Name = "Tsurane Sanzu-watashi", Input = "2,3,6,2,3,!6S" },
                new CharacterMove { Name = "Burst Tsurane Sanzu-watashi", Input = "2,3,6,2,3,!6D" },
                new CharacterMove { Name = "Metsudo Kushoudou (Why)", Input = "2,3,6,3,2,1,4,2,3,!6KH" }
            }
        },
        new Preset
        {
            Character = Character.Answer,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Business Ninpo: Under the Rug (Parry)", Input = "2,5,!2P" },
                new CharacterMove { Name = "Business Ninpo: Under the Rug (Parry) JI YRC Air Backdash", Input = "2,5,2,!8P,5*30,5PKS,5*20,4,5,4" },
                new CharacterMove { Name = "Dead Stock Ninpo: Firesale", Input = "6,3,2,1,4,!6D" }
            }
        }
    };
}

