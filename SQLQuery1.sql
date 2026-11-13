-- 1. Nos aseguramos de que el rol "Admin" exista en la base de datos
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'administrator@golbet.com')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID())
END

-- 2. Buscamos el ID de tu usuario y el ID del rol Admin
DECLARE @UserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email = 'administrator@golbet.com');
DECLARE @RoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'administrator@golbet.com');

-- 3. Le asignamos el rol de Administrador a tu usuario
IF @UserId IS NOT NULL AND @RoleId IS NOT NULL 
AND NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@UserId, @RoleId)
    PRINT '¡Usuario promovido a Administrator con éxito!'
END
ELSE
BEGIN
    PRINT 'No se encontró el usuario. Verifica que el correo esté bien escrito.'
END