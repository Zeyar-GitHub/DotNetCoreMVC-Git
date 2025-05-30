-- Insert Admin account (password: admin123)
INSERT INTO LogIn (UserName, Password, RoleId)
VALUES ('admin', '$2a$11$YwXxXxXxXxXxXxXxXxXxO', 1);
 
-- Insert User account (password: user123)
INSERT INTO LogIn (UserName, Password, RoleId)
VALUES ('user', '$2a$11$YwXxXxXxXxXxXxXxXxXxO', 2); 