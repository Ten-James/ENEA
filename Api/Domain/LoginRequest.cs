﻿namespace Domain;

public record LoginRequest(
    string Email,
    string Password
);