stream = File.stream!("input.txt")

numbers =
  Enum.map(stream, &String.trim/1)
  |> Enum.map(fn line -> String.graphemes(line) |> Enum.map(&String.to_integer/1) end)

defmodule Day3 do
  def get_largest_digits(bank, 1) do
    Enum.max(bank)
  end

  def get_largest_digits(bank, num) do
    first_digit = Enum.max(Enum.drop(bank, 1 - num))
    i = Enum.find_index(bank, fn x -> x == first_digit end)
    rest = Enum.drop(bank, i + 1)
    first_digit * 10 ** (num - 1) + get_largest_digits(rest, num - 1)
  end
end

Enum.map(numbers, fn bank ->
  Day3.get_largest_digits(bank, 2)
end)
|> Enum.sum()
|> IO.inspect()

Enum.map(numbers, fn bank ->
  Day3.get_largest_digits(bank, 12)
end)
|> Enum.sum()
|> IO.inspect()
