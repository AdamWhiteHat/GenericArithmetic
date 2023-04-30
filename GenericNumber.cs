using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedArithmetic
{
	public class GenericNumber<T>
		: IEquatable<GenericNumber<T>>, IComparable<GenericNumber<T>>, IComparable, IComparer<GenericNumber<T>>, IComparer, IFormattable
	{
		public T Value { get; set; }

		public GenericNumber(T value)
		{
			Value = value;
		}

		#region Implicit Conversion Casts

		public static implicit operator T(GenericNumber<T> source) => source.Value;
		public static implicit operator GenericNumber<T>(T source) => new GenericNumber<T>(source);

		#endregion

		#region Unary Operator Overloads

		public static GenericNumber<T> operator +(GenericNumber<T> a)
			=> a;
		public static GenericNumber<T> operator -(GenericNumber<T> a)
			=> new GenericNumber<T>(GenericArithmetic<T>.Negate(a.Value));

		#endregion

		#region Binary Operator Overloads

		public static GenericNumber<T> operator +(GenericNumber<T> left, GenericNumber<T> right)
			=> GenericArithmetic<T>.Add(left, right);
		public static GenericNumber<T> operator -(GenericNumber<T> left, GenericNumber<T> right)
			=> GenericArithmetic<T>.Subtract(left, right);
		public static GenericNumber<T> operator *(GenericNumber<T> left, GenericNumber<T> right)
			=> GenericArithmetic<T>.Multiply(left, right);
		public static GenericNumber<T> operator /(GenericNumber<T> left, GenericNumber<T> right)
			=> GenericArithmetic<T>.Divide(left, right);
		public static GenericNumber<T> operator %(GenericNumber<T> left, GenericNumber<T> right)
			=> GenericArithmetic<T>.Modulo(left, right);
		// Too confusing? ^ normally means XOR in C#. Making it Pow is probably unexpected behavior. 
		//public static GenericNumber<T> operator ^(GenericNumber<T> left, GenericNumber<T> right)
		//	=> GenericArithmetic<T>.Power(left, right);

		#endregion

		#region Equality Operator Overloads

		public static bool operator ==(GenericNumber<T> left, GenericNumber<T> right)
			=> GenericArithmetic<T>.Equal(left, right);
		public static bool operator !=(GenericNumber<T> left, GenericNumber<T> right)
			=> GenericArithmetic<T>.NotEqual(left, right);
		public static bool operator >(GenericNumber<T> left, GenericNumber<T> right)
			=> GenericArithmetic<T>.GreaterThan(left, right);
		public static bool operator <(GenericNumber<T> left, GenericNumber<T> right)
			=> GenericArithmetic<T>.LessThan(left, right);
		public static bool operator >=(GenericNumber<T> left, GenericNumber<T> right)
			=> GenericArithmetic<T>.GreaterThanOrEqual(left, right);
		public static bool operator <=(GenericNumber<T> left, GenericNumber<T> right)
			=> GenericArithmetic<T>.LessThanOrEqual(left, right);

		#endregion

		#region Overloads

		public int Compare(GenericNumber<T> x, GenericNumber<T> y)
		{
			if (!GenericArithmetic<T>.Equal(x, y))
			{
				return (GenericArithmetic<T>.LessThan(x, y)) ? -1 : 1;
			}
			return 0;
		}

		public int Compare(object x, object y)
		{
			GenericNumber<T> castedX = x as GenericNumber<T>;
			GenericNumber<T> castedY = y as GenericNumber<T>;
			if (castedX == null)
			{
				return (castedY == null) ? 0 : -1;
			}
			if (castedY == null)
			{
				return 1;
			}
			return Compare(castedX, castedY);
		}

		public int CompareTo(GenericNumber<T> other)
		{
			return Compare(this, other);
		}

		public int CompareTo(object obj)
		{
			GenericNumber<T> castedOther = obj as GenericNumber<T>;
			if (castedOther == null) { return 1; }
			return this.CompareTo(castedOther);
		}

		public bool Equals(GenericNumber<T> other)
		{
			if (other == null) { return false; }
			return GenericArithmetic<T>.Equal(this.Value, other.Value);
		}

		public override bool Equals(object obj)
		{
			GenericNumber<T> castedOther = obj as GenericNumber<T>;
			if (castedOther == null) { return false; }
			return this.Equals(castedOther);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return GenericArithmetic<T>.ToString(Value, formatProvider);
		}

		public override string ToString()
		{
			return GenericArithmetic<T>.ToString(Value);
		}

		#endregion

	}
}
