defmodule Day5 do
  defp is_in_range(n, {range_start, range_end}) do
    range_start <= n and n <= range_end
  end

  def part1(items, ranges) do
    Enum.count(items, fn item ->
      Enum.any?(ranges, fn range -> is_in_range(item, range) end)
    end)
  end

  def sort_ranges(ranges) do
    Enum.sort_by(ranges, fn {x, _} -> x end)
  end

  def part2(ranges) do
    sorted = sort_ranges(ranges)

    # merge overlapping ranges but without deleting any. [{1, 3}, {2, 5}] -> [{1, 3}, {1, 5}]
    {merged, _} =
      Enum.map_reduce(sorted, {0, 0}, fn {s, e}, {prev_s, prev_e} ->
        if s <= prev_e do
          # we need to merge
          new_interval = {prev_s, max(e, prev_e)}
          {new_interval, new_interval}
        else
          {{s, e}, {s, e}}
        end
      end)

    # Now we just need to delete ranges that start on the same number, but reverse first so that we keep the largest merged range
    merged
    |> Enum.reverse()
    |> Enum.uniq_by(fn {s, _} -> s end)
    |> Enum.sum_by(fn {s, e} -> e - s + 1 end)
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
Day5.part2(ranges) |> IO.inspect()
