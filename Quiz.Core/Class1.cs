using System.Drawing;

namespace Quiz.Core
{
	public enum AntRole
	{
		Warrior,
		Queen,
	}

	public interface IAnt
	{
		int Team { get; }

		AntRole Role { get; }

		Point Position { get; }

		int Strength { get; }
	}

	public readonly record struct AntCreate(AntRole Role, Point Position);

	public interface IState
	{
		IReadOnlyCollection<IAnt> Ants { get; }
	}

	public enum Course
	{
		Left,
		Top,
		Right,
		Bottom,
	}

	public enum Action
	{
		Move,
		Attack,
	}

	public readonly record struct Command(Course Course, Action Action, IAnt Ant);

	public interface IBot
	{
		IEnumerable<AntCreate> Init(Size placeSize);

		Command GetCommandStep(IState state, int team);
	}

	public sealed class GamePlay
	{
		public GamePlay(IBot left, IBot right)
		{
			ArgumentNullException.ThrowIfNull(left);
			ArgumentNullException.ThrowIfNull(right);

			Left = left;
			Right = right;
		}

		private IBot Left { get; }

		private IBot Right { get; }

		private StateCurrent? State { get; set; }

		public void Init(Size halfSize)
		{
			if (State is null)
			{
				State = new([Left.Init(halfSize), Right.Init(halfSize)]);
			}
		}

		public void Step()
		{
			Step(Left, 0);
			Step(Right, 1);
		}

		private void Step(IBot bot, int team)
		{
			var command = bot.GetCommandStep(State, team);
			var ant = State.ItemValid(command.Ant);
			switch (command.Action)
			{
				case Action.Move:
					State.Move(ant, command.Course);
					break;
				case Action.Attack:
					State.Move(ant, command.Course);
					break;
				default: throw Tools.EnumNotSupport(command.Action);
			}
		}

		private sealed class StateCurrent : IState
		{
			public StateCurrent(IEnumerable<IEnumerable<AntCreate>> teams)
			{
				ArgumentNullException.ThrowIfNull(teams);

				var teamCounter = 0;
				foreach (var team in teams)
				{
					foreach (var ant in team)
					{
						Ants.Add(new(ant, teamCounter));
					}

					teamCounter++;
				}
			}

			public Size PlaceSize { get; }

			public HashSet<Ant> Ants { get; } = [];

			public bool Contains(Point point) => new Rectangle(Point.Empty, PlaceSize).Contains(point);

			public Ant ItemValid(IAnt ant)
			{
				ArgumentNullException.ThrowIfNull(ant);

				if (ant is Ant curstomAnt)
				{
					if (Ants.Contains(curstomAnt))
					{
						return curstomAnt;
					}
					else
					{
						throw new InvalidOperationException();
					}
				}
				else
				{
					throw new InvalidOperationException();
				}
			}

			public Ant GetAnt(Point position) => Ants.Single(x => x.Position == position);

			public Ant? GetAntOrDefault(Point position) => Ants.SingleOrDefault(x => x.Position == position);

			public bool Move(Ant ant, Course course)
			{
				var newPosition = Tools.Offset(ant.Position, course);

				if (Contains(newPosition) && GetAntOrDefault(newPosition) is null)
				{
					ant.Position = newPosition;
					return true;
				}
				else
				{
					return false;
				}
			}

			public bool Attack(Ant current, Course course)
			{
				var attackPoint = Tools.Offset(current.Position, course);

				if (Contains(attackPoint) && GetAntOrDefault(attackPoint) is { } victim)
				{
					victim.Strength -= 1;
					if (victim.Strength == 0)
					{
						Ants.Remove(victim);
					}

					return true;
				}
				else
				{
					return false;
				}
			}

			IReadOnlyCollection<IAnt> IState.Ants => Ants;
		}

		private sealed class Ant : IAnt
		{
			public Ant(AntCreate creator, int team)
			{
				Team = team;
				Role = creator.Role;
				Position = creator.Position;
			}

			public int Team { get; }

			public AntRole Role { get; }

			public Point Position { get; set; }

			public int Strength { get; set; }
		}
	}

	internal static class Tools
	{
		public static Point Offset(Point point, Course course) => course switch
		{
			Course.Left => new(point.X - 1, point.Y),
			Course.Top => new(point.X, point.Y - 1),
			Course.Right => new(point.X + 1, point.Y),
			Course.Bottom => new(point.X, point.Y + 1),
			_ => throw EnumNotSupport(course),
		};

		public static Exception EnumNotSupport<T>(T value)
			where T : struct
			=> new InvalidOperationException($"Enum {typeof(T)} not supported value: {Convert.ToInt32(value)}");
	}
}
