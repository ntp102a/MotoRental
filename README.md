# Update database
Scaffold-DbContext "Server=.;Database=Rental_motorbike;Integrated Security=true;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force

# Tạo branch mới trước khi commit một feature mới
