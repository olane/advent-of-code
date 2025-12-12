defmodule Day6 do
  def part1(lines) do
    Enum.zip(lines)
    |> Enum.sum_by(fn {a, b, c, d, o} ->
      calcLine([a, b, c, d], o)
    end)
  end

  def calcLine(list, "*") do
    Enum.reduce(list, 1, fn i, acc ->
      {n, _} = Integer.parse(i)
      n * acc
    end)
  end

  def calcLine(list, "+") do
    Enum.reduce(list, 0, fn i, acc ->
      {n, _} = Integer.parse(i)
      n + acc
    end)
  end

  def getNum(string_list) do
    {a, _} = Integer.parse(String.trim(Enum.join(string_list)))
    a
  end

  def part2(cols) do
    {result, _, _} =
      Enum.reduce(cols, {0, 0, "*"}, fn
        {"", "", "", "", ""}, {total, this_sum, sign} ->
          {total + this_sum, 0, sign}

        {" ", " ", " ", " ", " "}, {total, this_sum, sign} ->
          {total + this_sum, 0, sign}

        {a, b, c, d, " "}, {total, this_sum, "+"} ->
          {total, getNum([a, b, c, d]) + this_sum, "+"}

        {a, b, c, d, " "}, {total, this_sum, "*"} ->
          {total, getNum([a, b, c, d]) * this_sum, "*"}

        {a, b, c, d, new_sign}, {total, _this_sum, _sign} ->
          {total, getNum([a, b, c, d]), new_sign}
      end)

    result
  end
end

lines =
  for line <-
        File.read!("input.txt")
        |> String.split("\n") do
    String.split(line, ~r{\s}, trim: true)
  end

columns =
  for line <-
        File.read!("input.txt")
        |> String.split("\n") do
    String.split(line, "")
  end
  |> Enum.zip()

lines
|> Day6.part1()
|> IO.inspect()

columns
|> Day6.part2()
|> IO.inspect()
