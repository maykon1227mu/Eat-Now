drop database dbtcm;
create database dbtcm;
use dbtcm;

create table tbusuario(
CodUsu int primary key auto_increment,
Nome varchar(80) not null,
Email varchar(40) not null unique,
Usuario varchar(40) not null,
Senha varchar(16) not null,
Tipo varchar(28) default "Cliente"
);

create table tbfuncionario(
CodFunc int primary key auto_increment,
Salario decimal(9,2) not null,
foreign key (CodFunc) references tbusuario(CodUsu) on delete cascade
);

create table tbfornecedor(
CodFor int primary key auto_increment,
CNPJ varchar(20) not null,
foreign key (CodFor) references tbusuario(CodUsu) on delete cascade
);

create table tbproduto(
CodProd int primary key auto_increment,
NomeProd varchar(40) not null,
Descricao varchar(80) not null,
Preco decimal(9,2) not null,
Qtd int unsigned not null,
UserId int not null,
Imagem mediumblob not null,
CategoriaId int not null,
Vendas int not null default 0,
Avaliacoes int not null default 0
);

create table tbcategoria(
CodCat int primary key auto_increment,
Categoria varchar(40) not null
);

create table tbpedido(
CodPed int primary key auto_increment,
ProdutoId int not null,
UserId int not null,
QtdPed int unsigned not null,
PrecoPed decimal(9,2) not null,
DataPed datetime default current_timestamp
);

create table tbPromocao(
PromoId int primary key auto_increment,
NomePromo varchar(80) not null,
Porcentagem tinyint not null,
data_exclusao datetime not null,
CategoriaId int
);

create TABLE tbPromocaoItem(
PromoIdItem INT PRIMARY KEY AUTO_INCREMENT,
ProdutoId INT NOT NULL,
PromoId INT NOT NULL,
Porcentagem tinyint not null,
PrecoPromo DECIMAL(9,2) NOT NULL,
data_exclusao DATETIME NOT NULL,
FOREIGN KEY (ProdutoId) REFERENCES tbproduto(CodProd),
FOREIGN KEY (PromoId) REFERENCES tbPromocao(PromoId)
);

create table tbcarrinho (
Id int primary key auto_increment,
UserId int not null,
ProdutoId int not null,
Quantidade int unsigned not null,
PrecoCar decimal(9,2) not null,
foreign key (ProdutoId) references tbproduto(CodProd)
);

create table tbcomentario(
ComentId int primary key auto_increment,
UserId int not null,
ProdutoId int not null,
Comentario varchar(255),
DataComent datetime default	current_timestamp,
Avaliacao int not null
);

alter table tbcomentario add constraint FK_UserIdComentario foreign key (UserId) references tbusuario(CodUsu);
alter table tbcomentario add constraint FK_ProdutoIdComentario foreign key (ProdutoId) references tbproduto(CodProd);

select * from tbusuario;

select tbpedido.CodPed, tbpedido.ProdutoId, tbpedido.UserId, tbpedido.DataPed, tbproduto.NomeProd, tbproduto.Preco, tbpedido.QtdPed from tbpedido join tbproduto on tbpedido.ProdutoId = tbproduto.CodProd where tbpedido.UserId = 1;

alter table tbpedido add constraint FK_ProdutoIdPedido foreign key (ProdutoId) references tbproduto(CodProd);
alter table tbpedido add constraint FK_UserIdPedido foreign key (UserId) references tbusuario(CodUsu);
alter table tbpromocao add constraint FK_CategoriaIdPromocao foreign key (CategoriaId) references tbcategoria(CodCat);

delimiter $$
create procedure spInserirCarrinho(vUserId int, vProdutoId int , vQuantidade int)
begin
declare vPreco decimal(9,2); 
if(select Qtd from tbproduto where CodProd = vProdutoId != 0) then
	if not exists(select * from tbcarrinho where UserId = vUserId and ProdutoId = vProdutoId) then
		if exists(select PrecoPromo from tbpromocaoitem where ProdutoId = vProdutoId) then
			set vPreco := (select PrecoPromo from tbpromocaoitem where ProdutoId = vProdutoId);
            insert into tbcarrinho (UserId, ProdutoId, Quantidade, PrecoCar) values (vUserId, vProdutoId, vQuantidade, vPreco);
            update tbproduto set Qtd = Qtd - vQuantidade where CodProd = vProdutoId;
		else
			set vPreco := (select PrecoPromo from tbpromocaoitem where ProdutoId = vProdutoId);
			insert into tbcarrinho (UserId, ProdutoId, Quantidade, PrecoCar) values (vUserId, vProdutoId, vQuantidade, vPreco);
			update tbproduto set Qtd = Qtd - vQuantidade where CodProd = vProdutoId;
        end if;
	else 
    update tbcarrinho set Quantidade = Quantidade + vQuantidade where UserId = vUserId and ProdutoId = vProdutoId;
    update tbproduto set Qtd = Qtd - vQuantidade where CodProd = vProdutoId;
    end if;
end if;
end
$$

delimiter ;

delimiter $$

create procedure spExcluirDoCarrinho(vUserId int, vProdutoId int , vQuantidade int)
begin
declare vQtd int;
set vQtd := (select Quantidade from tbcarrinho where UserId = vUserId and ProdutoId = vProdutoId);
if(vQtd <= 0 or vQtd = 1)then
	delete from tbcarrinho where UserId = vUserId and ProdutoId = vProdutoId;
	update tbproduto set Qtd = Qtd + vQuantidade where CodProd = vProdutoId;
    else
    update tbcarrinho set Quantidade = Quantidade - vQuantidade where UserId = vUserId and ProdutoId = vProdutoId;
    update tbproduto set Qtd = Qtd + vQuantidade where CodProd = vProdutoId;
    end if;
end
$$

delimiter ;

delimiter $$

create procedure spZerosCarrinho()
begin
	delete from tbcarrinho where Quantidade = 0;
end
$$

delimiter ;

delimiter $$

create procedure spLogin(vUsuario varchar(40), vSenha varchar(16))
begin
	select * from tbusuario where Email = vUsuario and Senha = vSenha;
end;
$$

delimiter ;

delimiter $$
create procedure spExcluirPromocao(vPromocaoId int)
begin
	delete from tbpromocaoitem where PromoId = vPromocaoId;
    delete from tbpromocao where PromoId = vPromocaoId;
end$$
delimiter ;

delimiter $$
create procedure spFinalizarCompra(vUserId int, vProdutoId int, vQtdVenda int, vVenda int)
begin
	declare vPreco decimal(9,2);
	if exists(select ProdutoId from tbpromocaoitem where ProdutoId = vProdutoId) then
		set vPreco := (select PrecoPromo from tbpromocaoitem where ProdutoId = vProdutoId);
		insert into tbpedido (ProdutoId, UserId, PrecoPed, QtdPed) values (vProdutoId, vUserId, vPreco, vQtdVenda);
        update tbproduto set Vendas = Vendas + vVenda where CodProd = vProdutoId;
	else
		set vPreco := (select Preco from tbproduto where CodProd = vProdutoId);
        insert into tbpedido (ProdutoId, UserId, PrecoPed, QtdPed) values (vProdutoId, vUserId, vPreco, vQtdVenda);
        update tbproduto set Vendas = Vendas + vVenda where CodProd = vProdutoId;
	end if;
end$$
delimiter ;

delimiter $$

create procedure spCadastrarUsuario(vNome varchar(80), vEmail varchar(40), vUsuario varchar(40), vSenha varchar(16))
begin
	insert into tbusuario(Nome, Email, Usuario, Senha, Tipo) values (vNome, vEmail, vUsuario, vSenha, "Cliente");
end
$$

delimiter ;

delimiter $$

create procedure spCadastrarFuncionario(vNome varchar(80), vEmail varchar(40), vUsuario varchar(40), vSenha varchar(16), vSalario decimal(9,2))
begin
	insert into tbusuario(Nome, Email, Usuario, Senha, Tipo) values (vNome, vEmail, vUsuario, vSenha, "Funcionario");
    insert into tbfuncionario(CodFunc, Salario) values (last_insert_id(), vSalario);
end
$$

delimiter ;

delimiter $$

create procedure spCadastrarFornecedor(vNome varchar(80), vEmail varchar(40), vUsuario varchar(40), vSenha varchar(16), vCNPJ varchar(20))
begin
	insert into tbusuario(Nome, Email, Usuario, Senha, Tipo) values (vNome, vEmail, vUsuario, vSenha, "Fornecedor");
    insert into tbfornecedor(CodFor, CNPJ) values (last_insert_id(), vCNPJ);
end
$$

delimiter ;

DELIMITER $$

create PROCEDURE spInserirPromocao(
    vNomePromo VARCHAR(80), 
    vPorcentagem TINYINT, 
    vCategoria VARCHAR(80), 
    vdata_exclusao DATETIME
)
BEGIN
    DECLARE vPromoId INT;
    declare vCategoriaId int;
    declare vPorcentagemCerta decimal(3,2);
    set vPorcentagemCerta := concat("0.", vPorcentagem);

    -- Verificar a categoria e inserir na tbPromocaoItem
    IF vCategoria = "Todos" THEN
		INSERT INTO tbPromocao (NomePromo, Porcentagem, data_exclusao)
		VALUES (vNomePromo, vPorcentagem, vdata_exclusao);
        SET vPromoId = LAST_INSERT_ID();
        -- Inserir todos os itens de todos os produtos
        INSERT INTO tbPromocaoItem (ProdutoId, PromoId, Porcentagem, PrecoPromo, data_exclusao)
        SELECT p.CodProd, vPromoId, vPorcentagem, (p.Preco - (p.Preco * vPorcentagemCerta)), vdata_exclusao 
        FROM tbProduto p;
    ELSE
		set vCategoriaId := (select CodCat from tbcategoria where Categoria = vCategoria);
		INSERT INTO tbPromocao (NomePromo, Porcentagem, data_exclusao, CategoriaId)
		VALUES (vNomePromo, vPorcentagem, vdata_exclusao, vCategoriaId);
        SET vPromoId = LAST_INSERT_ID();
        -- Inserir itens de uma categoria específica
        INSERT INTO tbPromocaoItem (ProdutoId, PromoId, Porcentagem, PrecoPromo, data_exclusao)
        SELECT p.CodProd, vPromoId, vPorcentagem, (p.Preco - (p.Preco * vPorcentagemCerta)), vdata_exclusao
        FROM tbProduto p
        JOIN tbCategoria c ON p.CategoriaId = c.CodCat
        WHERE c.Categoria = vCategoria;
    END IF;

END$$

DELIMITER ;

select sum(vendas * preco) from tbproduto;

select * from tbpromocao;
select * from tbpromocaoitem;


alter table tbproduto add constraint FK_UserId_tbProduto foreign key (UserId) references tbfornecedor(CodUsu);
alter table tbproduto add constraint FK_CategoriaId_tbProduto foreign key (CategoriaId) references tbcategoria(CodCat);

insert into tbcategoria (Categoria) values ("Comida Japonesa");
insert into tbcategoria (Categoria) values ("Comida Italiana");
insert into tbcategoria (Categoria) values ("Pizza");
insert into tbcategoria (Categoria) values ("Massas");
insert into tbcategoria (Categoria) values ("Hamburguer");
insert into tbcategoria (Categoria) values ("Aperitivos");
insert into tbcategoria (Categoria) values ("Sorvete");
insert into tbcategoria (Categoria) values ("Milkshake");
insert into tbcategoria (Categoria) values ("Açai");
insert into tbcategoria (Categoria) values ("Bebidas");

call spCadastrarUsuario("Admin", "admin@gmail.com", "Admin1", "12345");
update tbusuario set tipo = "Administrador" where codusu = 1;
call spCadastrarFornecedor("nome da empresa real","fornecedorteste@gmail.com", "Fornecedor teste", "12345", "00.623.904/0001-73");
call spCadastrarFuncionario("funcionarioteste", "funcionarioteste@gmail.com", "functeste", "12345", 1);
call spCadastrarUsuario("Nathan", "nathanbs1227@gmail.com", "Nathanbsy", "12345");

select * from tbusuario join tbfornecedor where tipo = "Fornecedor";
select * from tbusuario join tbfuncionario where tipo = "Funcionario";
select * from tbusuario;
select * from tbcarrinho;
select * from tbproduto;
select * from tbcategoria;