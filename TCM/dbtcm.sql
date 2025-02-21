drop database dbtcm;
create database dbtcm;
use dbtcm;

create table tbusuario(
CodUsu int primary key auto_increment,
Nome varchar(80) not null,
Email varchar(40) not null unique,
Usuario varchar(40) not null,
Senha varchar(16) not null,
FotoPerfil mediumblob,
Tipo varchar(28) default "Cliente"
);

create table tbfuncionario(
CodFunc int primary key auto_increment,
Salario decimal(9,2) not null,
UserId int not null,
foreign key (CodFunc) references tbusuario(CodUsu) on delete cascade,
foreign key (UserId) references tbusuario(CodUsu) on delete cascade
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
Avaliacoes int not null default 0,
Nota decimal(3,2) not null default 0
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
DataPed datetime default current_timestamp,
StatusPed varchar(150) default "Pagamento Aprovado"
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

DELIMITER $$

CREATE PROCEDURE spLogin(vUsuario VARCHAR(40), vSenha VARCHAR(16))
BEGIN
    DECLARE vId INT;
    SET vId := (SELECT CodUsu FROM tbusuario WHERE Email = vUsuario AND Senha = vSenha);

    IF vId IS NOT NULL THEN
        -- Verifica se o usuário é um fornecedor
        IF EXISTS (SELECT 1 FROM tbfornecedor WHERE CodFor = vId) THEN
            SELECT u.CodUsu, u.Nome, u.Email, u.Usuario, u.Senha, u.Tipo, 
                   f.CodFor, f.CNPJ 
            FROM tbusuario u
            JOIN tbfornecedor f ON u.CodUsu = f.CodFor
            WHERE u.CodUsu = vId;

        -- Verifica se o usuário é um funcionário
        ELSEIF EXISTS (SELECT 1 FROM tbfuncionario WHERE CodFunc = vId) THEN
            SELECT u.CodUsu, u.Nome, u.Email, u.Usuario, u.Senha, u.Tipo, 
                   f.CodFunc, f.Salario, f.UserId 
            FROM tbusuario u
            JOIN tbfuncionario f ON u.CodUsu = f.CodFunc
            WHERE u.CodUsu = vId;

        -- Caso contrário, retorna apenas os dados do usuário normal
        ELSE
            SELECT u.CodUsu, u.Nome, u.Email, u.Usuario, u.Senha, u.Tipo 
            FROM tbusuario u
            WHERE u.CodUsu = vId;
        END IF;
    ELSE
        -- Retorna um resultado vazio se o login falhar
        SELECT NULL AS CodUsu, NULL AS Nome, NULL AS Email, NULL AS Usuario, 
               NULL AS Senha, NULL AS Tipo;
    END IF;
END $$

DELIMITER ;

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

create procedure spCadastrarFuncionario(vNome varchar(80), vEmail varchar(40), vUsuario varchar(40), vSenha varchar(16), vSalario decimal(9,2), vUserId int)
begin
	insert into tbusuario(Nome, Email, Usuario, Senha, UserId, Tipo) values (vNome, vEmail, vUsuario, vSenha, vUserId, "Funcionario");
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

delimiter $$

create procedure spComentar(vUserId int, vProdutoId int, vComentario varchar(255), vAvaliacao int)
begin
declare vNota double;
	insert into tbcomentario (UserId, ProdutoId, Comentario, Avaliacao) values (vUserId, vProdutoId, vComentario, vAvaliacao); 
    update tbproduto set avaliacoes = avaliacoes + 1 where CodProd = vProdutoId;
    set vNota := (select sum(tbcomentario.avaliacao / tbproduto.avaliacoes) from tbcomentario join tbproduto where ProdutoId = vProdutoId);
    update tbproduto set nota = vNota;
end
$$

delimiter ;

delimiter $$

create procedure spExcluirComentario(vComentarioId int)
begin
declare vNota double;
declare vProdutoId int;
set vProdutoId := (select ProdutoId from tbcomentario where ComentId);
	delete from tbcomentario where ComentId = vComentarioId;
    update tbproduto set avaliacoes = avaliacoes - 1 where CodProd = vProdutoId;
    set vNota := (select sum(tbcomentario.avaliacao / tbproduto.avaliacoes) from tbcomentario join tbproduto where ProdutoId = vProdutoId);
    update tbproduto set nota = vNota;
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

select sum(tbcomentario.avaliacao / tbproduto.avaliacoes) from tbcomentario join tbproduto where ProdutoId = 1;

select sum(vendas * preco) from tbproduto;

select * from tbpromocao;
select * from tbpromocaoitem;


alter table tbproduto add constraint FK_UserId_tbProduto foreign key (UserId) references tbusuario(CodUsu);
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
call spCadastrarFuncionario("funcionarioteste", "funcionarioteste@gmail.com", "functeste", "12345", 1, 1);
call spCadastrarUsuario("Nathan", "nathanbs1227@gmail.com", "Nathanbsy", "12345");

select * from tbusuario join tbfornecedor where tipo = "Fornecedor";
select * from tbusuario join tbfuncionario where tipo = "Funcionario";
select * from tbusuario;
select * from tbcarrinho;
select * from tbproduto;
select * from tbcategoria;
select * from tbcomentario;

SELECT SUM(Avaliacao) FROM tbcomentario WHERE ProdutoId = 1;

