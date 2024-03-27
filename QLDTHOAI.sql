use master
--drop database QLDTHOAI;
create database QLDTHOAI
go

use QLDTHOAI;

--drop table PHONE
create table PHONE(
	ID int identity(1,1) PRIMARY KEY,
	NAME varchar(50),
	MANUFACTURER_ID int,
	THUMBNAIL varchar(1000),
	PRICE FLOAT
);

create table MANUFACTURER(
	ID int identity(1,1) PRIMARY KEY,
	NAME varchar(50)
);

create table CUSTOMER(
	CUS_ID int identity(1,1) PRIMARY KEY,
	FIRSTNAME nvarchar(40),
	LASTNAME nvarchar(40),
	GENDER nvarchar(10),
	PHONENUM nvarchar(12),
	CUS_ADDRESS nvarchar(100),
	DOB date,
	CUS_IMAGE varchar(1000)
);

create table ORDERS(
	ORDER_ID int identity(1,1) PRIMARY KEY,
	CUSTOMER_ID int foreign key references CUSTOMER(CUS_ID),
	CREATED_DATE datetime,
	TOTAL FLOAT,
	DISCOUNTS FLOAT
);

create table ORDERS_PHONE(
	ORDER_ID int,
	PHONE_ID int,
	PHONE_COUNT int,
	TOTAL float

	CONSTRAINT ORDERS_PHONE_PK PRIMARY KEY (ORDER_ID,PHONE_ID),
	CONSTRAINT ORDERS_PHONE_PK_FK1 foreign key (ORDER_ID) references ORDERS(ORDER_ID),
	CONSTRAINT ORDERS_PHONE_PK_FK2 foreign key (PHONE_ID) references PHONE(ID)
);

create table PROMOTIONS(
	PROMO_ID int identity(1,1) PRIMARY KEY,
	PROMO_NAME nvarchar(100),
	STARTDATE datetime,
	ENDDATE datetime,
	PROMO_MANUFACTURER_ID int,
	PROMO_PHONE_ID int,
	DISCOUNTS FLOAT,
	PROMO_STATUS varchar(15)

	constraint FK_PROMO_MANUFACTURER foreign key (PROMO_MANUFACTURER_ID) references MANUFACTURER(ID),
	constraint FK_PROMO_PHONE foreign key (PROMO_PHONE_ID) references PHONE(ID),
);

create table PROMO_ORDERS(
	PROMO_ID int foreign key references PROMOTIONS(PROMO_ID),
	ORDER_ID int foreign key references ORDERS(ORDER_ID),

	CONSTRAINT PK_PROMO_ORDERS PRIMARY KEY(PROMO_ID,ORDER_ID)
);


alter table PHONE add constraint FK_PHONE_MANUFACTURER foreign key (MANUFACTURER_ID) references MANUFACTURER(ID);


insert into MANUFACTURER
values	('Xiaomi'),
		('Samsung'),
		('LG'),
		('Apple')

insert into PHONE
values	('W41',(select ID from MANUFACTURER where NAME = 'LG'),'./Images/Phone00.jpg',66.6),
		('K62',(select ID from MANUFACTURER where NAME = 'LG'),'./Images/Phone01.jpg',69.69),
		('Xiaomi 14',(select ID from MANUFACTURER where NAME = 'Xiaomi'),'./Images/Phone02.jpg',2),
		('Xiaomi Pad 6S Pro 12.4',(select ID from MANUFACTURER where NAME = 'Xiaomi'),'./Images/Phone03.jpg',37),
		('Xiaomi Poco X6',(select ID from MANUFACTURER where NAME = 'Xiaomi'),'./Images/Phone04.jpg',50),
		('iPhone 15 Pro Max',(select ID from MANUFACTURER where NAME = 'Apple'),'./Images/Phone05.jpg',669),
		('iPhone SE (2022)',(select ID from MANUFACTURER where NAME = 'Apple'),'./Images/Phone06.jpg',669),
		('Samsung Galaxy S24 Ultra',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone07.jpg',669),
		('Samsung Galaxy Xcover7',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone08.jpg',669),
		('Samsung Galaxy Tab S9 FE+',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone09.jpg',669),
		('Samsung Galaxy Tab S10 E-',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone09.jpg',669),
		('iPhone 4 S Pro Max+',(select ID from MANUFACTURER where NAME = 'Apple'),'./Images/Phone09.jpg',669),
		('Note 11',(select ID from MANUFACTURER where NAME = 'Xiaomi'),'./Images/Phone09.jpg',669),
		('Note 10',(select ID from MANUFACTURER where NAME = 'Xiaomi'),'./Images/Phone09.jpg',669),
		('iPhone 6S',(select ID from MANUFACTURER where NAME = 'Apple'),'./Images/Phone09.jpg',669),
		('iPad 4',(select ID from MANUFACTURER where NAME = 'Apple'),'./Images/Phone09.jpg',669),
		('Samsung Galaxy S23 Ultra',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone09.jpg',669),
		('Samsung Galaxy S22 Ultra',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone09.jpg',669),
		('Samsung Galaxy S21',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone09.jpg',669),
		('iPhone 4S',(select ID from MANUFACTURER where NAME = 'Apple'),'./Images/Phone09.jpg',669),
		('iPhone 6+',(select ID from MANUFACTURER where NAME = 'Apple'),'./Images/Phone09.jpg',669),
		('K64',(select ID from MANUFACTURER where NAME = 'LG'),'./Images/Phone01.jpg',69.69),
		('OLE86',(select ID from MANUFACTURER where NAME = 'LG'),'./Images/Phone01.jpg',69.69),
		('Q24',(select ID from MANUFACTURER where NAME = 'LG'),'./Images/Phone01.jpg',69.69),
		('Note 7+',(select ID from MANUFACTURER where NAME = 'Xiaomi'),'./Images/Phone09.jpg',669),
		('Note 11+',(select ID from MANUFACTURER where NAME = 'Xiaomi'),'./Images/Phone09.jpg',669),
		('Gamming Phone 2022',(select ID from MANUFACTURER where NAME = 'Xiaomi'),'./Images/Phone09.jpg',669),
		('Samsung Galaxy Xcover6',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone08.jpg',669),
		('Samsung Galaxy Xcover5',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone08.jpg',669),
		('Samsung Galaxy Tab S20',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone08.jpg',669),
		('Samsung Galaxy Tab S23',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone08.jpg',669),
		('Samsung Galaxy Tab A10 Thunderbolt',(select ID from MANUFACTURER where NAME = 'Samsung'),'./Images/Phone08.jpg',669)
