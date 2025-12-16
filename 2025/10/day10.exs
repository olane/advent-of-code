Mix.install([{:bitmap, "~> 1.0"}])

defmodule Day10 do
  def parse(str) do
    str
    |> String.split("\n")
    |> Enum.filter(&(&1 != ""))
    |> Enum.map(fn str ->
      [target, buttons, joltages] =
        Regex.run(~r/\[(.*)] (.*) \{(.*)\}/, str, capture: :all_but_first)

      parsed_buttons =
        String.split(buttons, " ")
        |> Enum.map(fn button_str ->
          button_str
          |> String.slice(1..-2//1)
          |> String.split(",")
          |> Enum.map(&String.to_integer(&1))
        end)

      parsed_joltages =
        String.split(joltages, ",")
        |> Enum.map(&String.to_integer(&1))

      target_bitmap =
        String.graphemes(target)
        |> Enum.with_index()
        |> Enum.reduce(Bitmap.new(String.length(target)), fn
          {"#", i}, acc -> Bitmap.set(acc, i)
          {".", _}, acc -> acc
        end)

      %{target: target_bitmap, buttons: parsed_buttons, joltages: parsed_joltages}
    end)
  end

  def toggle(states, button) do
    Enum.reduce(button, states, fn elem, acc -> Bitmap.toggle(acc, elem) end)
  end

  def search(machine, q, visited) do
    {{_, {current, s}}, q} = :queue.out(q)

    if(current == machine.target) do
      s
    else
      neighbors =
        for button <- machine.buttons,
            new_state = toggle(current, button),
            not MapSet.member?(visited, new_state) do
          {new_state, s + 1}
        end

      new_visited =
        Enum.reduce(neighbors, visited, fn {state, _}, acc -> MapSet.put(acc, state) end)

      search(machine, :queue.join(q, :queue.from_list(neighbors)), new_visited)
    end
  end

  def solve_machine(machine) do
    q = :queue.new()
    start = Bitmap.unset_all(machine.target)
    q = :queue.in({start, 0}, q)
    visited = MapSet.new([start])
    search(machine, q, visited)
  end

  def part1(machines) do
    Enum.sum_by(machines, &solve_machine/1)
  end
end

File.read!("input.txt")
|> Day10.parse()
|> Day10.part1()
|> IO.inspect()
