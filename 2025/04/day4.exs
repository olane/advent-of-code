defmodule Grid do
  def from_nested_list(rows) do
    for {row, y} <- Enum.with_index(rows),
        {val, x} <- Enum.with_index(row),
        into: %{} do
      {{x, y}, val}
    end
  end

  def from_string(string, mapping_fun \\ &Function.identity/1) do
    string
    |> String.trim()
    |> String.split("\n")
    |> Enum.map(fn line ->
      line |> String.graphemes() |> Enum.map(mapping_fun)
    end)
    |> from_nested_list()
  end

  def neighbour_positions({x, y}) do
    for dx <- -1..1, dy <- -1..1, {dx, dy} != {0, 0} do
      {dx + x, dy + y}
    end
  end

  def is_neighbour({x1, y1}, {x2, y2}) do
    abs(x1 - x2) <= 1 and abs(y1 - y2) <= 1 and (x1 != x2 or y1 != y2)
  end

  def neighbours(grid, pos) do
    Map.filter(grid, fn {key, _} -> is_neighbour(key, pos) end)
  end
end

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
