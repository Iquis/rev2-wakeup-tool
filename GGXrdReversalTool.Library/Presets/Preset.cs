using System.Collections.Immutable;
using GGXrdReversalTool.Library.Domain.Characters;

namespace GGXrdReversalTool.Library.Presets;

public class Preset
{
    public ICharacter RelatedCharacter { get; }

    public ImmutableList<CharacterMove> CharacterMoves { get; }

    private Preset(ICharacter character, ImmutableList<CharacterMove> characterMoves) =>
        (RelatedCharacter, CharacterMoves) = (character, characterMoves);
    private Preset(ICharacter character, params CharacterMove[] characterMoves) =>
        (RelatedCharacter, CharacterMoves) = (character, ImmutableList.Create(characterMoves));

    private Preset(params CharacterMove[] characterMoves) 
        :this(ICharacter.Zero, characterMoves)
    {
    }

    public static IEnumerable<Preset> Presets => new[]
    {
        new Preset(
            new CharacterMove("Blitz High", "!5SH"),
            new CharacterMove("Blitz Low", "!2SH"),
            new CharacterMove("Backdash", "4,5,!4"),
            new CharacterMove("Backdash into standing FD", "4,5,!4,5*10,4PK*200"),
            new CharacterMove("Backdash into crouching FD", "4,5,!4,5*10,1PK*200"),
            new CharacterMove("Backdash YRC", "yrc 4,5,!4,5*5,5PKS"),
            new CharacterMove("FD Jump", "!7,1PK*200"),
            new CharacterMove("FD Superjump", "!2,7,1PK*200"),
            new CharacterMove("Gold Burst", "!5DH"),
            new CharacterMove("Throw + 6P OS", "!6PH*3"),
            new CharacterMove("Throw + cS OS", "!6SH*3")
        ),
        new Preset(
            Character.Sol,
            new CharacterMove("Gun Flame", "2,3,!6P"),
            new CharacterMove("Gun Flame (Feint)", "2,1,!4P"),
            new CharacterMove("Volcanic Viper H (DP)", "6,2,!3H"),
            new CharacterMove("Volcanic Viper S (DP)", "6,2,!3S"),
            new CharacterMove("Tyrant Rave", "6,3,2,1,4,!6H"),
            new CharacterMove("Burst Tyrant Rave", "6,3,2,1,4,!6D")
        ),
        new Preset(
            Character.Ky,
            new CharacterMove("Stun Edge", "2,3,!6S"),
            new CharacterMove("Charged Stun Edge", "2,3,!6H"),
            new CharacterMove("Vapor Thrust H (DP)", "6,2,!3H"),
            new CharacterMove("Vapor Thrust S (DP)", "6,2,!3S"),
            new CharacterMove("Ride The Lightning", "6,3,2,1,4,!6H"),
            new CharacterMove("Burst Ride The Lightning", "6,3,2,1,4,!6D")
        ),
        new Preset(
            Character.May,
            new CharacterMove("Ultimate Spinning Whirlwind", "6,3,2,1,!4H"),
            new CharacterMove("Great Yamada Attack", "2,3,6,2,3,!6S"),
            new CharacterMove("Burst Great Yamada Attack", "2,3,6,2,3,!6D")
        ),
        new Preset(
            Character.Millia,
            new CharacterMove("Winger", "2,1,4,1,2,3,!6H"),
            new CharacterMove("Burst Winger", "2,1,4,1,2,3,!6D")
        ),
        new Preset(
            Character.Zato,
            new CharacterMove("Amorphous", "6,3,2,1,4,!6H"),
            new CharacterMove("Burst Amorphous", "6,3,2,1,4,!6D")
        ),
        new Preset(
            Character.Potemkin,
            new CharacterMove("Giganter Kai", "6,3,2,1,4,!6H"),
            new CharacterMove("Giganter Bullet Kai", "6,3,2,1,4,!6H,5*42,4,1,2,3,6,4,1,2,3,6P"),
            new CharacterMove("Heavenly Potemkin Buster", "2,3,6,2,3,!6S"),
            new CharacterMove("Burst Heavenly Potemkin Buster", "2,3,6,2,3,!6S"),
            new CharacterMove("Backdash Buster", "4,5,!4,5*13,6,3,2,1,4,6P")
        ),
        new Preset(
            Character.Chipp, 
            new CharacterMove("Beta Blade (DP)", "6,2,!3S"), 
            new CharacterMove("Zansei Rouga", "6,3,2,1,4,!6H"),
            new CharacterMove("Burst Zansei Rouga", "6,3,2,1,4,!6D"), 
            new CharacterMove("Banki Mesai", "2,3,6,2,3,!6K")
            ),
        new Preset(
            Character.Faust, 
            new CharacterMove("Stimulating Fists of Annihilation", "2,3,6,2,3,!6S"), 
            new CharacterMove("Burst Stimulating Fists of Annihilation", "2,3,6,2,3,!6D"), 
            new CharacterMove("5D", "!5D")
            ),
        new Preset(
            Character.Axl, 
            new CharacterMove("Artemis Hunter AKA Benten (DP)", "6,2,!3S"), 
            new CharacterMove("Sickle Storm", "2,3,6,3,2,1,!4H"),
            new CharacterMove("Burst Sickle Storm", "2,3,6,3,2,1,!4D")
            ),
        new Preset(
            Character.Venom, 
            new CharacterMove("Burst Dark Angel", "2,3,6,3,2,1,!4D")
            ),
        new Preset(
            Character.Slayer,
            new CharacterMove("BDC K Mappa Hunch", "4,5,!4*4,2,3,6,9K"), 
            new CharacterMove("BDC P Mappa Hunch", "4,5,!4*4,2,3,6,9K"),
            new CharacterMove("BDC Bloodsucking Universe (Grab)", "6,3,2,1,4,5,!4*4,7H"),
            new CharacterMove("BDC Dead on Time", "6,3,2,1,4,5,!4*4,6,9S"), 
            new CharacterMove("BDC Burst Dead on Time", "6,3,2,1,4,5,!4*4,6,9D"),
            new CharacterMove("Burst Dead on Time", "6,3,2,1,4,!6D"), 
            new CharacterMove("BDC P Dandy Step -> Pilebunker", "2,1,4,5,!4*4,7P,5*15,5P"),
            new CharacterMove("Backdash Jump FD", "4,5,!4*5,7,1PK*50"), 
            new CharacterMove("Eternal Wings", "2,3,6,2,3,!6H")
            ),
        new Preset(
            Character.Ino, 
            new CharacterMove("Longing Desperation", "6,3,2,1,4,!6H"), 
            new CharacterMove("Burst Longing Desperation", "6,3,2,1,4,!6D"),
            new CharacterMove("Vertical Chemical Love YRC", "2,1,!4S,5*5,5PKS")
            ),
        new Preset(
            Character.Bedman, 
            new CharacterMove("Sinusoidal Helios", "6,3,2,1,4,!6H"),
            new CharacterMove("Burst Sinusoidal Helios", "6,3,2,1,4,!6D")
            ),
        new Preset(
            Character.Ramlethal,
            new CharacterMove("Burst Calvados (Need Swords Equipped)", "6,3,2,1,4,!6D"),
            new CharacterMove("Explode", "2,3,6,3,2,1,!4K"), 
            new CharacterMove("Sildo Detruo", "2,1,!4K")
            ),
        new Preset(
            Character.Sin,
            new CharacterMove("Hawk Baker (DP)", "6,2,!3S"),
            new CharacterMove("Hawk Baker into Elk Hunt", "6,2,!3S,5*5,2,3,6K"),
            new CharacterMove("R.T.L", "6,3,2,1,4,!6H"),
            new CharacterMove("Burst R.T.L", "6,3,2,1,4,!6D")
            ),
        new Preset(
            Character.Elphelt, 
            new CharacterMove("Judge Better Half", "2,3,6,2,3,!6S"), 
            new CharacterMove("Burst Judge Better Half", "2,3,6,2,3,!6D")
            ),
        new Preset(
            Character.Leo, 
            new CharacterMove("Eisen Sturm H (Flashkick) (SET DUMMY TO CROUCHING OR IT WONT WORK)", "!8H"),
            new CharacterMove("Eisen Sturm S (Flashkick) (SET DUMMY TO CROUCHING OR IT WONT WORK)", "!8S"),
            new CharacterMove("Leidenschaft Dirigent", "6,3,2,1,4,!6H")
            ),
        new Preset(
            Character.Johnny, 
            new CharacterMove("That's My Name (J)", "2,3,6,3,2,1,!4H"),
            new CharacterMove("Burst That's My Name (J)", "2,3,6,3,2,1,!4D")
            ),
        new Preset(
            Character.JackO, 
            new CharacterMove("2D (DP)", "!2D"), 
            new CharacterMove("Forever Elysion Driver", "4,1,2,3,6,9,8,!7P"), 
            new CharacterMove("Burst Calvados", "2,1,!4D")
            ),
        new Preset(
            Character.Jam, 
            new CharacterMove("Parry -> P", "4,5,!6,5P"), 
            new CharacterMove("Parry -> PP", "4,5,!6,5P,5*31,5P,5,5P,5,5P,5,5P"),
            new CharacterMove("Parry -> PK", "4,5,!6,5P,5*31,5K,5,5K,5,5K,5,5K"),
            new CharacterMove("Parry FD Cancel on Whiff", "4,5,!6,5*4,1PK*200"),
            new CharacterMove("Kenroukaku (DP)", "6,2,!3K"),
            new CharacterMove("Kenroukaku (DP) into Gekirin", "6,2,!3K,5*10,2,1,4K"),
            new CharacterMove("Bao Saishinshou", "2,3,6,2,3,!6H"), 
            new CharacterMove("Choukyaku Hou'Oushou", "6,3,2,1,4,!6S"),
            new CharacterMove("Burst Choukyaku Hou'Oushou", "6,3,2,1,4,!6D")
            ),
        new Preset(
            Character.Haehyun,
            new CharacterMove("Blue Tuning Ball", "2,3,6S"),
            new CharacterMove("Red Tuning Ball", "2,3,6H"),
            new CharacterMove("Four Tigers Sword", "6,2,3K"),
            new CharacterMove("Four Tigers Sword [Max]", "6,2,3K,5K*56"),
            new CharacterMove("Four Tigers Sword (Reverse)", "6,2,3K,4K*6"), 
            new CharacterMove("Four Tigers Sword (Reverse) [Max]", "6,2,3K,4K*19"), 
            new CharacterMove("Blue Shi-Shiinken", "6,2,3k*10,5*7,4k*12"), 
            new CharacterMove("Red Shi-Shiinken", "6,2,3k*10,5*7,4k*25"), 
            new CharacterMove("Falcon Dive", "2,1,4K"), 
            new CharacterMove("Falcon Dive [Max]", "2,1,4K"), 
            new CharacterMove("Falcon Dive (Reverse)", "2,1,4K*7"),
            new CharacterMove("Falcon Dive (Reverse) [Max]", "2,1,4K*7,5k*59"),
            new CharacterMove("Air Falcon Dive", "2,1,4,7*5,7k"),
            new CharacterMove("Enlightened 3000 Palm Strike", "2,3,6,2,3,!6H"),
            new CharacterMove("Burst Enlightened 3000 Palm Strike", "2,3,6,2,3,!6D"),
            new CharacterMove("Enlightened 3000 Palm Strike [Max]", "2,3,6,2,3,6H,5*80,5H*195"), 
            new CharacterMove("Celestial Tuning Ball", "2,3,6,2,3,6S")
            ),
        new Preset(
            Character.Raven, 
            new CharacterMove("Getreuer", "2,1,4,2,1,!4H"), 
            new CharacterMove("Burst Getreuer", "2,1,4,2,1,!4D"), 
            new CharacterMove("Stance", "2,1,!4K*50")
            ),
        new Preset(
            Character.Dizzy,
            new CharacterMove("Don't be overprotective (Mirror)", "6,3,2,1,4,!6P"),
            new CharacterMove("Burst Imperial Ray", "6,3,2,1,4,!6D")
            ),
        new Preset(
            Character.Baiken,
            new CharacterMove("Azami -> Kuchinashi", "!5HS,5P"),
            new CharacterMove("Azami -> Mawarikomi", "!5HS,5K"),
            new CharacterMove("Azami -> Sakura", "!5HS,5S"),
            new CharacterMove("Azami -> Rokkonsogi", "!5HS,5H"),
            new CharacterMove("Low Azami -> Kuchinashi", "!2HS,5P"), 
            new CharacterMove("Low Azami -> Mawarikomi", "!2HS,5K"), 
            new CharacterMove("Low Azami -> Sakura", "!2HS,5S"), 
            new CharacterMove("Low Azami -> Rokkonsogi", "!2HS,5H"), 
            new CharacterMove("Tsurane Sanzu-watashi", "2,3,6,2,3,!6S"), 
            new CharacterMove("Burst Tsurane Sanzu-watashi", "2,3,6,2,3,!6D"), 
            new CharacterMove("Metsudo Kushoudou (Why)", "2,3,6,3,2,1,4,2,3,!6KH")
            ),
        new Preset(
            Character.Answer, 
            new CharacterMove("Business Ninpo: Under the Rug (Parry)", "2,5,!2P"),
            new CharacterMove("Business Ninpo: Under the Rug (Parry) JI YRC Air Backdash", "2,5,2,!8P,5*30,5PKS,5*20,4,5,4"), 
            new CharacterMove("Dead Stock Ninpo: Firesale", "6,3,2,1,4,!6D")
            )
    };
    
}


