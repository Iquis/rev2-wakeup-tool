namespace GGXrdReversalTool.Library.Domain.Characters;

public class Character : IEquatable<Character>
{
    public CharacterName CharName { get; }
    public int FaceUpFrames { get; }
    public int FaceDownFrames { get; }

    public int WallSplatWakeupTiming => 15;

    private Character(string charName, int faceUpFrames, int faceDownFrames)
    {
        CharName = charName;
        FaceUpFrames = faceUpFrames;
        FaceDownFrames = faceDownFrames;
    }

    public bool Equals(Character? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return string.Equals(CharName, other.CharName) &&
               FaceUpFrames == other.FaceUpFrames &&
               FaceDownFrames == other.FaceDownFrames;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj.GetType() != GetType())
        {
            return false;
        }
        return Equals((Character)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = CharName.GetHashCode();
            hashCode = (hashCode * 397) ^ FaceUpFrames;
            hashCode = (hashCode * 397) ^ FaceDownFrames;
            return hashCode;
        }
    }


    public static Character Sol = new("Sol", 25, 21);
    public static Character Ky = new("Ky", 23, 21);
    public static Character May = new("May", 25, 22);
    public static Character Millia = new("Millia", 25, 23);
    public static Character Zato = new("Zato", 25, 22);
    public static Character Potemkin = new("Potemkin", 24, 22);
    public static Character Chipp = new("Chipp", 30, 24);
    public static Character Faust = new("Faust", 25, 29);
    public static Character Axl = new("Axl", 25, 21);
    public static Character Venom = new("Venom", 21, 26);
    public static Character Slayer = new("Slayer", 26, 20);
    public static Character I_No = new("I-No", 24, 20);
    public static Character Bedman = new("Bedman", 24, 30);
    public static Character Ramlethal = new("Ramlethal", 25, 23);
    public static Character Sin = new("Sin", 30, 21);
    public static Character Elphelt = new("Elphelt", 27, 27);
    public static Character Leo = new("Leo", 28, 26);
    public static Character Johnny = new("Johnny", 25, 24);
    public static Character Jack_O = new("Jack-O", 25, 23);
    public static Character Jam = new("Jam", 26, 25);
    public static Character Haehyun = new("Haehyun", 22, 27);
    public static Character Raven = new("Raven", 25, 24);
    public static Character Dizzy = new("Dizzy", 25, 24);
    public static Character Baiken = new("Baiken", 25, 21);
    public static Character Answer = new("Answer", 25, 25);
    public static List<Character> Characters = new()
    {
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
        I_No,
        Bedman,
        Ramlethal,
        Sin,
        Elphelt,
        Leo,
        Johnny,
        Jack_O,
        Jam,
        Haehyun,
        Raven,
        Dizzy,
        Baiken,
        Answer
    };
}