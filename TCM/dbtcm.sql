create database dbtcm;
use dbtcm;

create table tbusuario(
CodUsu int primary key auto_increment,
email varchar(40),
usuario varchar(40),
senha varchar(16)
);

create table tbproduto(
CodProd int primary key auto_increment,
NomeProd varchar(40),
Descricao varchar(80),
Preco decimal(9,2)
);

select * from tbproduto;