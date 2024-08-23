using System.Collections.Immutable;

namespace GGXrdReversalTool.Library.Domain.Characters;

public record Character 
{
    public CharacterName CharName { get; }
    public int FaceUpFrames { get; }
    public int FaceDownFrames { get; }

    public int WallSplatWakeupTiming => 15;

    private Character(string charName, int faceUpFrames, int faceDownFrames) =>
        (CharName, FaceUpFrames, FaceDownFrames) = (charName, faceUpFrames, faceDownFrames);
     

    public static readonly Character Sol = new("Sol", 25, 21);
    public static readonly Character Ky = new("Ky", 23, 21);
    public static readonly Character May = new("May", 25, 22);
    public static readonly Character Millia = new("Millia", 25, 23);
    public static readonly Character Zato = new("Zato", 25, 22);
    public static readonly Character Potemkin = new("Potemkin", 24, 22);
    public static readonly Character Chipp = new("Chipp", 30, 24);
    public static readonly Character Faust = new("Faust", 25, 29);
    public static readonly Character Axl = new("Axl", 25, 21);
    public static readonly Character Venom = new("Venom", 21, 26);
    public static readonly Character Slayer = new("Slayer", 26, 20);
    public static readonly Character Ino = new("I-No", 24, 20);
    public static readonly Character Bedman = new("Bedman", 24, 30);
    public static readonly Character Ramlethal = new("Ramlethal", 25, 23);
    public static readonly Character Sin = new("Sin", 30, 21);
    public static readonly Character Elphelt = new("Elphelt", 27, 27);
    public static readonly Character Leo = new("Leo", 28, 26);
    public static readonly Character Johnny = new("Johnny", 25, 24);
    public static readonly Character JackO = new("Jack-O", 25, 23);
    public static readonly Character Jam = new("Jam", 26, 25);
    public static readonly Character Haehyun = new("Haehyun", 22, 27);
    public static readonly Character Raven = new("Raven", 25, 24);
    public static readonly Character Dizzy = new("Dizzy", 25, 24);
    public static readonly Character Baiken = new("Baiken", 25, 21);
    public static readonly Character Answer = new("Answer", 25, 25);

    public static readonly ImmutableList<Character> Characters = ImmutableList.Create(
        Sol,
        Ky,
        May,
        Millia,
        Zato,
        Potemkin,
        Chipp,
        Faust,
        Axl,
        Venom,
        Slayer,
        Ino,
        Bedman,
        Ramlethal,
        Sin,
        Elphelt,
        Leo,
        Johnny,
        JackO,
        Jam,
        Haehyun,
        Raven,
        Dizzy,
        Baiken,
        Answer
    );
}