using System;

class KeyGenerator
{
    private static readonly Random rand = new Random();

    public string Create()
    {
        byte v1 = (byte)rand.Next(0, 255);
        byte v2 = (byte)rand.Next(0, 255);
        byte v3 = (byte)rand.Next(0, 255);

        // Format: XX XX XX 7C XX XX XX XX
        // Validator (sub_421850 in HTTPDebuggerUI.exe) requires:
        //   b1 == ~b3, b2 == b0 ^ b7, b4 == b3 ^ ~b6, b5 == b6 ^ 7
        return string.Format("{0:X2}{1:X2}{2:X2}7C{3:X2}{4:X2}{5:X2}{6:X2}",
            v1,
            v2 ^ 0x7C,
            (byte)~v1,
            v2,
            v3,
            v3 ^ 7,
            v1 ^ (byte)~v3);
    }
}
