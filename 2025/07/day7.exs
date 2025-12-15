Code.require_file("../util/grid.ex")

defmodule Day7 do
  # y is the height we're at, x_list is the list of positions we have beams in the row above
  def generate_beams(grid, y, x_list, beam_count) do
    if grid[{hd(x_list), y}] == nil do
      beam_count
    else
      new_list =
        x_list
        |> Enum.flat_map(fn x ->
          case grid[{x, y}] do
            "^" -> [x - 1, x + 1]
            _ -> [x]
          end
        end)

      splits = Enum.count(new_list) - Enum.count(x_list)

      generate_beams(grid, y + 1, Enum.uniq(new_list), beam_count + splits)
    end
  end

  def part_1(grid) do
    {x, y} = Grid.get_pos_of(grid, "S")
    beam_count = generate_beams(grid, y + 1, [x], 1)
    beam_count - 1
  end

  # y is the height we're at, x_list is the list of {xpos, pathcount} beams in the row above
  def generate_beams_2(grid, y, x_list) do
    {first_x, _} = hd(x_list)

    if grid[{first_x, y}] == nil do
      x_list
      |> Enum.sum_by(fn {_, c} -> c end)
    else
      new_list =
        x_list
        |> Enum.flat_map(fn {x, c} ->
          case grid[{x, y}] do
            "^" -> [{x - 1, c}, {x + 1, c}]
            _ -> [{x, c}]
          end
        end)
        |> Enum.group_by(fn {x, _} -> x end)
        |> Enum.map(fn {key, list} -> {key, Enum.sum_by(list, fn {_, c} -> c end)} end)

      generate_beams_2(grid, y + 1, new_list)
    end
  end

  def part_2(grid) do
    {x, y} = Grid.get_pos_of(grid, "S")
    generate_beams_2(grid, y + 1, [{x, 1}])
  end
end

File.read!("input.txt")
|> Grid.from_string()
|> Day7.part_1()
|> IO.inspect()

File.read!("input.txt")
|> Grid.from_string()
|> Day7.part_2()
|> IO.inspect()
