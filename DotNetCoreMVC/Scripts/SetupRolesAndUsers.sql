-- First, ensure Roles table has the necessary roles
IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleId = 1)
BEGIN
    INSERT INTO Roles (RoleId, RoleName) VALUES (1, 'Admin');
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleId = 2)
BEGIN
    INSERT INTO Roles (RoleId, RoleName) VALUES (2, 'User');
END

-- Create Michael's admin account if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM LogIn WHERE UserName = 'michael')
BEGIN
    INSERT INTO LogIn (UserName, Password, RoleId)
    VALUES ('michael', '$2a$11$YwXxXxXxXxXxXxXxXxXxO', 1);
END 