﻿using Microsoft.EntityFrameworkCore;

namespace MCStatusDSBot.Models;

[Index(nameof(ServerAddress), IsUnique = true)]
public class Observer
{
    public int Id { get; set; }

    public List<ObserverMessage> Messages { get; set; }
    public string ServerAddress { get; set; }
}