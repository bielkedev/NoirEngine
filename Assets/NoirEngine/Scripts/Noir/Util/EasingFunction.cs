using System;

namespace Noir.Util
{
	public delegate float EasingFunc(float nStart, float nEnd, float nFactor);

	/// <summary>
	/// 애니메이션에 사용되는 각종 커브 함수를 정의합니다.
	/// </summary>
	public class EasingFunction
	{
		public static EasingFunc Linear
		{
			get
			{
				return (float nStart, float nEnd, float nFactor) =>
				{
					return (nEnd - nStart) * nFactor + nStart;
				};
			}
		}

		public class Quadratic
		{
			public static EasingFunc In
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor * nFactor);
					};
				}
			}

			public static EasingFunc Out
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor * (2f - nFactor));
					};
				}
			}

			public static EasingFunc InOut
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, (nFactor *= 2f) < 1f ? 0.5f * nFactor * nFactor : -0.5f * ((nFactor -= 1f) * (nFactor - 2f) - 1f));
					};
				}
			}
		}

		public class Cubic
		{
			public static EasingFunc In
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor * nFactor * nFactor);
					};
				}
			}

			public static EasingFunc Out
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, 1f  + (nFactor -= 1f) * nFactor * nFactor);
					};
				}
			}

			public static EasingFunc InOut
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, (nFactor *= 2f) < 1f ? 0.5f * nFactor * nFactor * nFactor : 0.5f * ((nFactor -= 2f) * nFactor * nFactor + 2f));
					};
				}
			}
		}

		public class Quartic
		{
			public static EasingFunc In
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor * nFactor * nFactor * nFactor);
					};
				}
			}

			public static EasingFunc Out
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, 1f - (nFactor -= 1f) * nFactor * nFactor * nFactor);
					};
				}
			}

			public static EasingFunc InOut
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, (nFactor *= 2f) < 1f ? 0.5f * nFactor * nFactor * nFactor * nFactor : -0.5f * ((nFactor -= 2f) * nFactor * nFactor * nFactor - 2f));
					};
				}
			}
		}

		public class Quintic
		{
			public static EasingFunc In
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor * nFactor * nFactor * nFactor * nFactor);
					};
				}
			}

			public static EasingFunc Out
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, 1f + (nFactor -= 1f) * nFactor * nFactor * nFactor * nFactor);
					};
				}
			}

			public static EasingFunc InOut
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, (nFactor *= 2f) < 1f ? 0.5f * nFactor * nFactor * nFactor * nFactor * nFactor : 0.5f * ((nFactor -= 2f) * nFactor * nFactor * nFactor * nFactor + 2f));
					};
				}
			}
		}

		public class Sinusoidal
		{
			public static EasingFunc In
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, 1f - (float)Math.Cos(nFactor * Math.PI / 2f));
					};
				}
			}

			public static EasingFunc Out
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, (float)Math.Sin(nFactor * Math.PI / 2f));
					};
				}
			}

			public static EasingFunc InOut
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, 0.5f * (1f - (float)Math.Cos(Math.PI * nFactor)));
					};
				}
			}
		}

		public class Exponential
		{
			public static EasingFunc In
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor == 0f ? 0f : (float)Math.Pow(1024f, nFactor - 1f));
					};
				}
			}

			public static EasingFunc Out
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor == 1f ? 1f : 1f - (float)Math.Pow(2f, -10f * nFactor));
					};
				}
			}

			public static EasingFunc InOut
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor == 0f ? 0f : nFactor == 1f ? 1f : (nFactor *= 2f) < 1f ? 0.5f * (float)Math.Pow(1024f, nFactor - 1f) : 0.5f * (-(float)Math.Pow(2f, -10f * (nFactor - 1f)) + 2f));
					};
				}
			}
		}

		public class Circular
		{
			public static EasingFunc In
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, 1f - (float)Math.Sqrt(1f - nFactor * nFactor));
					};
				}
			}

			public static EasingFunc Out
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, (float)Math.Sqrt(1f - (nFactor -= 1f) * nFactor));
					};
				}
			}

			public static EasingFunc InOut
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, (nFactor *= 2f) < 1f ? -0.5f * ((float)Math.Sqrt(1f - nFactor * nFactor) - 1f) : 0.5f * ((float)Math.Sqrt(1f - (nFactor -= 2f) * nFactor) - 1f));
					};
				}
			}
		}

		public class Elastic
		{
			public static EasingFunc In
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor == 0f ? 0f : nFactor == 1f ? 1f : (float)(-Math.Pow(2f, 10f * (nFactor -= 1f)) * Math.Sin((nFactor - 0.1f) * 2f * Math.PI / 0.4f)));
					};
				}
			}

			public static EasingFunc Out
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor == 0f ? 0f : nFactor == 1f ? 1f : (float)(Math.Pow(2f, -10f * nFactor) * Math.Sin((nFactor - 0.1f) * 2f * Math.PI / 0.4f) + 1f));
					};
				}
			}

			public static EasingFunc InOut
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, (nFactor *= 2f) < 1f ? (float)(-0.5f * Math.Pow(2f, 10f * (nFactor -= 1f)) * Math.Sin((nFactor - 0.1f) * 2f * Math.PI / 0.4f)) : (float)(Math.Pow(2f, -10f * (nFactor -= 1f)) * Math.Sin((nFactor - 0.1f) * 2f * Math.PI / 0.4f) * 0.5f + 1f));
					};
				}
			}
		}

		public class Back
		{
			public const float S1 = 1.70158f;
			public const float S2 = 2.5949095f;

			public static EasingFunc In
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor * nFactor * ((Back.S1 + 1f) * nFactor - Back.S1));
					};
				}
			}

			public static EasingFunc Out
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, (nFactor -= 1f) * nFactor * ((Back.S1 + 1f) * nFactor + Back.S1) + 1f);
					};
				}
			}

			public static EasingFunc InOut
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, (nFactor *= 2f) < 1f ? 0.5f * nFactor * nFactor * ((Back.S2 + 1f) * nFactor - Back.S2) : 0.5f * ((nFactor -= 2f) * nFactor * ((Back.S2 + 1f) * nFactor + Back.S2) + 2f));
					};
				}
			}
		}

		public class Bounce
		{
			public const float K = 7.5625f;
			public const float S1 = 1f / 2.75f;
			public const float S2 = 1.5f / 2.75f;
			public const float S3 = 2f / 2.75f;
			public const float S4 = 2.25f / 2.75f;
			public const float S5 = 2.5f / 2.75f;
			public const float S6 = 2.625f / 2.75f;
			public const float A1 = 0.75f;
			public const float A2 = 0.9375f;
			public const float A3 = 0.984375f;

			public static EasingFunc In
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, 1f - Bounce.calcFactor(1f - nFactor));
					};
				}
			}

			public static EasingFunc Out
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, Bounce.calcFactor(nFactor));
					};
				}
			}

			public static EasingFunc InOut
			{
				get
				{
					return (float nStart, float nEnd, float nFactor) =>
					{
						return EasingFunction.Linear(nStart, nEnd, nFactor < 0.5f ? 0.5f * (1f - Bounce.calcFactor(1f - nFactor * 2f)) : Bounce.calcFactor(nFactor * 2f - 1f) * 0.5f + 0.5f);
					};
				}
			}

			private static float calcFactor(float nFactor)
			{
				if (nFactor < Bounce.S1)
					return Bounce.K * nFactor * nFactor;
				else if (nFactor < Bounce.S3)
					return Bounce.K * (nFactor -= Bounce.S2) * nFactor + Bounce.A1;
				else if (nFactor < Bounce.S5)
					return Bounce.K * (nFactor -= Bounce.S4) * nFactor + Bounce.A2;
				else
					return Bounce.K * (nFactor -= Bounce.S6) * nFactor + Bounce.A3;
			}
		}
	}
}