defmodule Day9 do
  def parse(str) do
    str
    |> String.split("\n")
    |> Enum.map(fn node_str ->
      [x, y] = String.split(node_str, ",") |> Enum.map(&String.to_integer(&1))
      %{x: x, y: y}
    end)
  end

  # Cross product of vectors (b-a) and (c-a)
  def cross(a, b, c) do
    (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)
  end

  # Check if segments properly cross (not just touch at endpoints)
  def intersect?(a, b, c, d) do
    d1 = cross(c, d, a)
    d2 = cross(c, d, b)
    d3 = cross(a, b, c)
    d4 = cross(a, b, d)

    # Strictly opposite signs means proper crossing
    ((d1 > 0 and d2 < 0) or (d1 < 0 and d2 > 0)) and
      ((d3 > 0 and d4 < 0) or (d3 < 0 and d4 > 0))
  end

  # checks poly1 and poly2 intersect
  def intersect_poly?(poly1, poly2) do
    for {a, b} <- poly1,
        {c, d} <- poly2 do
      intersect?(a, b, c, d)
    end
    |> Enum.any?()
  end

  def size(n1, n2) do
    (abs(n1.x - n2.x) + 1) * (abs(n1.y - n2.y) + 1)
  end

  def generate_poly(point_list) do
    Enum.scan(
      point_list,
      {0, hd(Enum.reverse(point_list))},
      fn element, {_, acc2} -> {acc2, element} end
    )
  end

  def generate_rect(%{x: n1x, y: n1y}, %{x: n2x, y: n2y}) do
    generate_poly([
      %{x: n1x, y: n1y},
      %{x: n1x, y: n2y},
      %{x: n2x, y: n2y},
      %{x: n2x, y: n1y}
    ])
  end

  # Check if point is on segment (a, b)
  def on_segment?(p, a, b) do
    min_x = min(a.x, b.x)
    max_x = max(a.x, b.x)
    min_y = min(a.y, b.y)
    max_y = max(a.y, b.y)

    p.x >= min_x and p.x <= max_x and p.y >= min_y and p.y <= max_y and
      (b.x - a.x) * (p.y - a.y) == (b.y - a.y) * (p.x - a.x)
  end

  # Check if horizontal ray from p going right crosses segment (a, b)
  def ray_crosses?(p, a, b) do
    {low, high} = if a.y <= b.y, do: {a, b}, else: {b, a}

    if p.y <= low.y or p.y > high.y do
      false
    else
      if low.y == high.y do
        false
      else
        x_cross = low.x + (high.x - low.x) * (p.y - low.y) / (high.y - low.y)
        x_cross > p.x
      end
    end
  end

  # Check if point is inside or on polygon using ray casting
  def point_in_polygon?(p, edges) do
    if Enum.any?(edges, fn {a, b} -> on_segment?(p, a, b) end) do
      true
    else
      crossings = Enum.count(edges, fn {a, b} -> ray_crosses?(p, a, b) end)
      rem(crossings, 2) == 1
    end
  end

  def part1(nodes) do
    for n1 <- nodes,
        n2 <- nodes,
        n1 != n2 do
      size(n1, n2)
    end
    |> Enum.max()
  end

  def part2(nodes) do
    outer_poly = generate_poly(nodes)

    for n1 <- nodes,
        n2 <- nodes,
        n1 != n2,
        not intersect_poly?(outer_poly, generate_rect(n1, n2)) do
      # The other two corners of the rectangle
      c3 = %{x: n1.x, y: n2.y}
      c4 = %{x: n2.x, y: n1.y}

      # Check corners are inside polygon AND no edges cross
      if point_in_polygon?(c3, outer_poly) and
           point_in_polygon?(c4, outer_poly) and
           not intersect_poly?(outer_poly, generate_rect(n1, n2)) do
        size(n1, n2)
      else
        0
      end
    end
    |> Enum.max()
  end
end

File.read!("input.txt")
|> Day9.parse()
|> Day9.part1()
|> IO.inspect()

File.read!("input.txt")
|> Day9.parse()
|> Day9.part2()
|> IO.inspect()
