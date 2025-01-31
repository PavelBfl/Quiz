using System.Drawing;

namespace Quiz.Core
{
	public enum AntRole
	{
		Warrior,
		Queen,
	}

	public readonly record struct AntTeamId(AntRole Role, int Id);

	public readonly record struct AntId(AntTeamId AntTeamId, int Team);

	public interface IAnt
	{
		AntId Id { get; }

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

	public readonly record struct Command(Course Course, Action Action, AntTeamId AntId);

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

		public void Step()
		{
			Step(Left, 0);
			Step(Right, 1);
		}

		private void Step(IBot bot, int team)
		{
			var command = bot.GetCommandStep(State, team);
			switch (command.Action)
			{
				case Action.Move:
					State.Move(new(command.AntId, team), command.Course);
					break;
				case Action.Attack:
					State.Move(new(command.AntId, team), command.Course);
					break;
				default: throw Tools.EnumNotSupport(command.Action);
			}
		}

		private sealed class StateCurrent : IState
		{
			private static void ThrowIfNotUnique<T>(IEnumerable<T> source)
			{
				ArgumentNullException.ThrowIfNull(source);

				var accumulator = new HashSet<T>();
				foreach (var item in source)
				{
					if (!accumulator.Add(item))
					{
						throw new InvalidOperationException();
					}
				}
			}

			public StateCurrent(IEnumerable<IEnumerable<AntCreate>> teams)
			{
				ArgumentNullException.ThrowIfNull(teams);

				var teamCounter = 0;
				foreach (var team in teams)
				{
					var antCounter = 0;
					foreach (var ant in team)
					{
						Ants.Add(new(ant, antCounter, teamCounter));
						antCounter++;
					}

					teamCounter++;
				}
			}

			public Size PlaceSize { get; }

			public List<Ant> Ants { get; } = [];

			public bool Contains(Point point) => new Rectangle(Point.Empty, PlaceSize).Contains(point);

			public Ant GetAnt(AntId id) => Ants.Single(x => x.Id == id);

			public Ant? GetAntOrDefault(AntId id) => Ants.SingleOrDefault(x => x.Id == id);

			public Ant GetAnt(Point position) => Ants.Single(x => x.Position == position);

			public Ant? GetAntOrDefault(Point position) => Ants.SingleOrDefault(x => x.Position == position);

			public bool Move(AntId id, Course course)
			{
				var ant = GetAnt(id);
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

			public bool Attack(AntId id, Course course)
			{
				var current = GetAnt(id);
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

			IReadOnlyCollection<IAnt> IState.Ants => Ants.AsReadOnly();
		}

		private sealed class Ant : IAnt
		{
			public Ant(AntCreate creator, int localId, int team)
			{
				Id = new(new(creator.Role, localId), team);
				Position = creator.Position;
			}

			public AntId Id { get; }

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
