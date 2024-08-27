using Knx.Falcon;

namespace KNXIntegrator.Models;

public class Converter
{

    public GroupValue IntToGroupValue(int value, int sizeInBit)
    {
        if (sizeInBit == 0){
            throw new ArgumentException("sizeInBit cannot be null");
        }
        else if (value > ((int)Math.Pow(2.0,(double)sizeInBit))-1){
            throw new ArgumentException("value is too big, does not fit in sizeInBit");
        }
        else if (sizeInBit == 1)
        {
            // If sizeInBit is 1, the value can only be 0 or 1, which corresponds to a boolean.
            return new GroupValue(value != 0);
        }
        else if (sizeInBit > 1 && sizeInBit <= 8)
        {
            if (sizeInBit == 8)
            {
                // If sizeInBit is 8, use the Byte constructor.
                return new GroupValue((byte)value);
            }
            else
            {
                // If sizeInBit is between 2 and 8, use the Byte and Int32 constructor.
                return new GroupValue((byte)value, sizeInBit);
            }
        }
        else if (sizeInBit > 8 && sizeInBit % 8 == 0)
        {
            // Calculate the number of bytes needed based on sizeInBit.
            int byteCount = sizeInBit / 8;

            // Create a byte array with the exact size needed.
            byte[] byteArray = new byte[byteCount];

            // Fill the byte array with the appropriate bits from the integer value.
            for (int i = 0; i < byteCount; i++)
            {
                byteArray[i] = (byte)(value >> (8 * (byteCount - 1 - i)));
            }

            return new GroupValue(byteArray);
        }
        else
        {

            throw new ArgumentException("Invalid size in bits. It must be a multiple of 8.");
        }
    }


}