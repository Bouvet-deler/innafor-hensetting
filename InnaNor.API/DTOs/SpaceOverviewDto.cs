﻿namespace InnaNor.API.DTOs;

public class SpaceOverviewDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
