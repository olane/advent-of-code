defmodule Day5 do
  defp is_in_range(item, {range_start, range_end}) do
    range_start <= item and item <= range_end
  end

  def part1(items, ranges) do
    Enum.count(items, fn item ->
      Enum.any?(ranges, fn range -> is_in_range(item, range) end)
    end)
  end
end

items =
  for line <-
        File.read!("input_items.txt")
        |> String.trim()
        |> String.split("\n") do
    {a, _} = Integer.parse(line)
    a
  end

ranges =
  for line <-
        File.read!("input_ranges.txt")
        |> String.trim()
        |> String.split("\n") do
    [{a, _}, {b, _}] = String.split(line, "-") |> Enum.map(&Integer.parse/1)
    {a, b}
  end

Day5.part1(items, ranges) |> IO.inspect()
