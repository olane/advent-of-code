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
    Map.new(neighbour_positions(pos), fn p -> {p, grid[p]} end)
    |> Map.filter(fn {_, v} -> !is_nil(v) end)
  end

  def get_pos_of(grid, char) do
    {pos, _} =
      grid
      |> Map.filter(fn {_, v} -> v == char end)
      |> Map.to_list()
      |> hd()

    pos
  end

  defp add_vectors({ax, ay}, {bx, by}) do
    {ax + bx, ay + by}
  end

  # creates an infinite stream of positions in the direction of the
  # vector starting from the start_pos (but not including it)
  defp position_stream(start_pos, vector) do
    Stream.unfold(add_vectors(start_pos, vector), fn
      pos -> {pos, add_vectors(pos, vector)}
    end)
  end

  # starts from start_pos, iterates in direction of vector, and
  # returns the first {position} that matches char.
  # returns {pos, :notfound} if we go out of bounds.
  def next_on_vector(grid, char, start_pos, vector) do
    position_stream(start_pos, vector)
    |> Enum.find_value(fn pos ->
      case grid[pos] do
        x when x == char -> pos
        nil -> {pos, :notfound}
        _ -> false
      end
    end)
  end
end
