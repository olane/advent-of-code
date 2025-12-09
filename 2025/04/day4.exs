Code.require_file("../util/grid.ex")

defmodule Day4 do
  def part_1(grid) do
    Map.filter(grid, fn {key, val} ->
      Grid.neighbours(grid, key)
      |> Enum.count(fn {_, neighbour_val} -> neighbour_val == "@" end) < 4 and val == "@"
    end)
    |> Enum.count()
  end
end

File.read!("input.txt")
|> Grid.from_string()
|> Day4.part_1()
|> IO.inspect()
