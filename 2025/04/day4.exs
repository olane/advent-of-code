Code.require_file("../util/grid.ex")

defmodule Day4 do
  def part_1(grid) do
    Map.filter(grid, fn {key, val} ->
      Grid.neighbours(grid, key)
      |> Enum.count(fn {_, neighbour_val} -> neighbour_val == "@" end) < 4 and val == "@"
    end)
    |> Enum.count()
  end

  def remove_removable(grid) do
    removable =
      Map.filter(grid, fn {key, val} ->
        Grid.neighbours(grid, key)
        |> Enum.count(fn {_, neighbour_val} -> neighbour_val == "@" end) < 4 and val == "@"
      end)

    new_grid = Map.drop(grid, Map.keys(removable))

    if map_size(new_grid) == map_size(grid) do
      new_grid
    else
      remove_removable(new_grid)
    end
  end

  def part_2(grid) do
    map_size(grid) - map_size(remove_removable(grid))
  end
end

File.read!("input.txt")
|> Grid.from_string()
|> Day4.part_1()
|> IO.inspect()

File.read!("input.txt")
|> Grid.from_string()
|> Day4.part_2()
|> IO.inspect()
