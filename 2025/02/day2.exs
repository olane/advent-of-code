stream = File.stream!("input.txt")

range_strings =
  Enum.map(stream, &String.trim/1)
  |> hd
  |> String.split(",")

defmodule Day2 do
  def get_range(string) do
    for s <- String.split(string, "-") do
      String.to_integer(s)
    end
  end

  def is_weird(num) do
    digits = Integer.digits(num)
    len = round(length(digits) / 2)
    Enum.take(digits, len) == Enum.drop(digits, len)
  end

  def is_weirder(num) do
    digits = Integer.digits(num)

    if length(digits) >= 2 do
      for len <- 1..div(length(digits), 2) do
        chunks = Enum.chunk_every(digits, len)
        all_equal(chunks)
      end
      |> Enum.any?()
    else
      false
    end
  end

  def all_equal([_]) do
    true
  end

  def all_equal([a, b]) do
    a == b
  end

  def all_equal([h, h2 | t]) do
    h == h2 and all_equal([h2 | t])
  end
end

range_strings
|> Enum.map(&Day2.get_range(&1))
|> Enum.map(fn
  [a, b] -> Enum.filter(a..b, &Day2.is_weird(&1)) |> Enum.sum()
end)
|> Enum.sum()
|> IO.inspect()

range_strings
|> Enum.map(&Day2.get_range(&1))
|> Enum.map(fn
  [a, b] -> Enum.filter(a..b, &Day2.is_weirder(&1)) |> Enum.sum()
end)
|> Enum.sum()
|> IO.inspect()
