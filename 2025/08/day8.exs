defmodule Day8 do
  def parse(str) do
    str
    |> String.split("\n")
    |> Enum.map(fn node_str ->
      [x, y, z] = String.split(node_str, ",") |> Enum.map(&String.to_integer(&1))
      %{x: x, y: y, z: z}
    end)
    |> Enum.with_index()
    |> Map.new(fn {n, i} -> {i, n} end)
  end

  def distance(%{x: x1, y: y1, z: z1}, %{x: x2, y: y2, z: z2}) do
    (x1 - x2) ** 2 + (y1 - y2) ** 2 + (z1 - z2) ** 2
  end

  def build_distance_list(nodes) do
    for i <- 0..(Enum.count(nodes) - 1),
        j <- 0..(Enum.count(nodes) - 1),
        i < j do
      {i, j, distance(nodes[i], nodes[j])}
    end
  end

  def group_contains_any?(group, item_list) do
    Enum.any?(group, fn x -> Enum.any?(item_list, fn y -> x == y end) end)
  end

  # merges the two groups that contain id1 and id2s
  def merge_groups(groups, id1, id2) do
    merged = Enum.filter(groups, fn g -> group_contains_any?(g, [id1, id2]) end) |> Enum.concat()
    rest = Enum.filter(groups, fn g -> !group_contains_any?(g, [id1, id2]) end)

    [merged | rest]
  end

  def merge_many_groups(groups, []) do
    groups
  end

  def merge_many_groups(groups, [{id1, id2, _} | rest]) do
    merge_many_groups(merge_groups(groups, id1, id2), rest)
  end

  def group_smallest_n(sorted_distance_list, n, groups) do
    smallest_n =
      sorted_distance_list
      |> Enum.take(n)

    merge_many_groups(groups, smallest_n)
    |> Enum.map(&Enum.count/1)
    |> Enum.sort(:desc)
    |> Enum.take(3)
    |> Enum.reduce(&(&1 * &2))
  end

  def gen_groups(n) do
    for i <- 0..(n - 1) do
      [i]
    end
  end

  def part1(nodes) do
    groups = gen_groups(map_size(nodes))

    sorted_distances =
      build_distance_list(nodes)
      |> Enum.sort_by(fn {_id1, _id2, distance} -> distance end)

    group_smallest_n(sorted_distances, 1000, groups)
  end

  def group_until_complete([next_to_merge | sorted_distance_list], nodes, groups) do
    {id1, id2, _} = next_to_merge

    if Enum.count(groups) == 2 and Enum.count(merge_groups(groups, id1, id2)) == 1 do
      # this is the final step, calculate answer
      nodes[id1].x * nodes[id2].x
    else
      # keep merging
      group_until_complete(sorted_distance_list, nodes, merge_groups(groups, id1, id2))
    end
  end

  def part2(nodes) do
    groups = gen_groups(map_size(nodes))

    sorted_distances =
      build_distance_list(nodes)
      |> Enum.sort_by(fn {_id1, _id2, distance} -> distance end)

    group_until_complete(sorted_distances, nodes, groups)
  end
end

File.read!("input.txt")
|> Day8.parse()
|> Day8.part1()
|> IO.inspect()

File.read!("input.txt")
|> Day8.parse()
|> Day8.part2()
|> IO.inspect()
