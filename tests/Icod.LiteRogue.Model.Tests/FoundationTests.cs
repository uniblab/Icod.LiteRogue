/*
	Icod.LiteRogue.Model.Tests
	Automated test suite for the Icod.LiteRogue domain model.
	Copyright (C) 2026  Timothy J. Bruce <uniblab@hotmail.com>
*/

/*
	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Icod.LiteRogue.Model;
using Icod.LiteRogue.Model.Presentation;

namespace Icod.LiteRogue.Model.Tests;

public sealed class FoundationTests {
	[Fact]
	public void NewRunStartsFresh() {
		var run = GameSession.NewRun(123, GameRules.Default);
		var view = run.Snapshot();
		Assert.Equal(1, view.Depth);
		Assert.Equal(RunStatus.Playing, view.Status);
		Assert.Equal(view.MaximumHealth, view.Health);
		Assert.Equal(0, view.Potions);
		Assert.Equal(4, view.Weapon.Damage);
		Assert.Equal(0, view.Armor.Reduction);
		Assert.Equal(123, view.Seed);
	}

	[Fact]
	public void SnapshotCannotMutateModel() {
		var run = GameSession.NewRun(123, GameRules.Default);
		var view = run.Snapshot();
		Assert.Throws<NotSupportedException>(() => ((IList<TileSnapshot>)view.Tiles)[0] = default);
		Assert.Equal(view.Tiles, run.Snapshot().Tiles);
	}

	[Fact]
	public void ModelDoesNotDependOnDCurses() {
		Assert.DoesNotContain(typeof(GameSession).Assembly.GetReferencedAssemblies(), a => a.Name!.StartsWith("Icod.DCurses", StringComparison.Ordinal));
	}
}
