defmodule Day9 do
  def parse(str) do
    str
    |> String.split("\n")
    |> Enum.map(fn node_str ->
      [x, y] = String.split(node_str, ",") |> Enum.map(&String.to_integer(&1))
      %{x: x, y: y}
    end)
  end

  def size(n1, n2) do
    (abs(n1.x - n2.x) + 1) * abs(n1.y - n2.y + 1)
  end

  def part1(nodes) do
    for n1 <- nodes,
        n2 <- nodes,
        n1 != n2 do
      size(n1, n2)
    end
    |> Enum.max()
  end
end

File.read!("input.txt")
|> Day9.parse()
|> Day9.part1()
|> IO.inspect()
