use DAN
create table "User"
(
	UId bigint primary key identity(1,1),
	Uname varchar(500) not null,
	Upass varchar(500)	not null,
	Roles varchar(500)
)
create table Category
(
	CId bigint primary key identity(1,1),
	Cname nvarchar(500) not null,
	Cdesc nvarchar(500) not null,
)
create table Product
(
	PId bigint primary key identity(1,1),
	Pname nvarchar(500) not null,
	Pdesc nvarchar(MAX) not null,
	Price money not null,
	Sale int default 0,
	SalePrice money not null,
	"View" bigint,
	CId bigint not null,
	Foreign key(CId) references Category(CId)
)
create table Image
(
	Id bigint identity(1,1) primary key,
	IType varchar(MAX) not null,
	IData varbinary(MAX) not null,
	PId bigint not null,
	Foreign key(PId) references Product(PId)
)
create table "Order"
(
	Id bigint identity(1,1) primary key,
	Prefix nvarchar(100) not null,
	OId as Prefix + CAST(Id as varchar(100)),
	Fullname nvarchar(500) not null,
	Address nvarchar(MAX) not null,
	Phone varchar(11) not null,
	Email varchar(500),
	TotalPrice money not null,
	Code varchar(500) not null,
	CreatOn DateTime default getdate() not null,
	Status bit default 0 not null,
	PaidInfo nvarchar(500),
	Paid bit default 0 not null
)
create table OrderDetail
(
	Id bigint identity(1,1) primary key,
	Quantum int not null,
	Price money not null,
	PId bigint not null,
	OId bigint not null,
	Foreign key(OId) references "Order"(Id)
)
create table Contact
(
	Id int identity(1,1) primary key,
	"From" nvarchar(500) not null,
	Subject nvarchar(MAX) not null,
	Message nvarchar(MAX) not null,
	"Read" bit default 0
)
create table Sale
(
	Id bigint identity(1,1) primary key,
	Title nvarchar(500) not null,
	Content nvarchar(MAX) not null,
	CreatOn DateTime
)
create table Code
(
	Id bigint identity(1,1) primary key,
	Code varchar(500) not null,
	Price money not null,
	Expired bit default 0
 )
 select  * from Sale