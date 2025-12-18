defmodule Day11 do
  def parse(str) do
    str
    |> String.split("\n")
    |> Enum.filter(&(&1 != ""))
    |> Enum.map(fn node_str ->
      [head | rest] = String.split(node_str, " ")
      {String.slice(head, 0..-2//1), rest}
    end)
    |> Map.new()
  end

  def find_paths(node_map, current, finish, cache \\ %{})

  def find_paths(_, any, any, cache) do
    {[[any]], cache}
  end

  def find_paths(node_map, current, finish, cache) do
    case Map.fetch(cache, {current, finish}) do
      {:ok, cached_paths} ->
        {cached_paths, cache}

      :error ->
        {paths, new_cache} =
          Enum.flat_map_reduce(node_map[current] || [], cache, fn next_node, acc_cache ->
            {sub_paths, updated_cache} = find_paths(node_map, next_node, finish, acc_cache)
            {Enum.map(sub_paths, &[current | &1]), updated_cache}
          end)

        {paths, Map.put(new_cache, {current, finish}, paths)}
    end
  end

  def part1(node_map) do
    {paths, _cache} = find_paths(node_map, "you", "out")
    Enum.count(paths)
  end

  def part2(node_map) do
    {paths1, _cache} = find_paths(node_map, "svr", "fft")
    {paths2, _cache} = find_paths(node_map, "fft", "dac")
    {paths3, _cache} = find_paths(node_map, "dac", "out")
    [paths1, paths2, paths3] |> Enum.product_by(&Enum.count/1)
  end
end

File.read!("input.txt")
|> Day11.parse()
|> Day11.part1()
|> IO.inspect()

File.read!("input.txt")
|> Day11.parse()
|> Day11.part2()
|> IO.inspect()
