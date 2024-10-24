using System;

namespace EDBTools.Common
{
    /// <summary>
    /// Represents a 32-bit variable of indeterminate type.
    /// Can represent a <see cref="float"/>, <see cref="int"/> or <see cref="uint"/>.
    /// </summary>
    public class EXVar32
    {
        private byte[] data;

        /// <summary>
        /// The unsigned integer representation of this variable.
        /// </summary>
        public uint U
        {
            get
            {
                return BitConverter.ToUInt32(data, 0);
            }
            set
            {
                this.data = BitConverter.GetBytes(value);
            }
        }

        /// <summary>
        /// The signed integer representation of this variable.
        /// </summary>
        public int I
        {
            get
            {
                return BitConverter.ToInt32(data, 0);
            }
            set
            {
                this.data = BitConverter.GetBytes(value);
            }
        }

        /// <summary>
        /// The single-precision floating-point representation of this variable.
        /// </summary>
        public float F
        {
            get
            {
                return BitConverter.ToSingle(data, 0);
            }
            set
            {
                this.data = BitConverter.GetBytes(value);
            }
        }

        /// <summary>
        /// Initializes the variable with the bytes contained in <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The provided data.</param>
        /// <exception cref="ArgumentException"></exception>
        public EXVar32(byte[] data)
        {
            if (data.Length != 4)
            {
                throw new ArgumentException("Provided byte array must be of size 4 to meet the 32-bit requirement.");
            }

            this.data = data;
        }

        /// <summary>
        /// Initializes the variable with the unsigned data of <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The provided data.</param>
        public EXVar32(uint data)
        {
            U = data;
        }

        /// <summary>
        /// Initializes the variable with the signed data of <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The provided data.</param>
        public EXVar32(int data)
        {
            I = data;
        }

        /// <summary>
        /// Initializes the variable with the floating point data of <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The provided data.</param>
        public EXVar32(float data)
        {
            F = data;
        }

        /// <summary>
        /// Initializes the variable with all bytes set to 0.
        /// </summary>
        public EXVar32()
        {
            U = 0;
        }

        /// <summary>
        /// Do rough tests on the data of this variable to guess the datatype.
        /// </summary>
        /// <returns>
        /// The <see cref="Type"/> of either <see cref="float"/>, <see cref="int"/> or <see cref="uint"/>.
        /// </returns>
        public Type GuessType()
        {
            int i = I;
            float f = F;

            if ((f < 1000000f) && (f > 0.000001f))
            {
                return typeof(float);
            } else if (i < 0)
            {
                return typeof(int);
            } else
            {
                return typeof(uint);
            }
        }

        /// <summary>
        /// Get a string representation of this variable using the guessed datatype.
        /// </summary>
        /// <returns>A string representation of this variable using the guessed datatype.</returns>
        public override string ToString()
        {
            Type T = GuessType();

            if (T == typeof(float))
            {
                return F.ToString() + "f";
            } else if (T == typeof(int))
            {
                return I.ToString();
            } else
            {
                uint u = U;

                //Represent in hexadecimal if the upper bits are non-zero
                if ((u & 0xFFFF0000) != 0)
                {
                    return u.ToString("X") + "h";
                } else
                {
                    return u.ToString();
                }
            }
        }
    }
}
