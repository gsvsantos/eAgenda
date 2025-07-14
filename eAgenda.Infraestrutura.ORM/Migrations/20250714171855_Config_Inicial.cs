using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAgenda.Infraestrutura.ORM.Migrations
{
    /// <inheritdoc />
    public partial class Config_Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBCategoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBCategoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBContato",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Cargo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Empresa = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBContato", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBDespesa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DataOcorrencia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FormaPagamento = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBDespesa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBTarefa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Prioridade = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PercentualConcluido = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBTarefa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBCompromisso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Assunto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DataOcorrencia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraTermino = table.Column<TimeSpan>(type: "interval", nullable: false),
                    TipoCompromisso = table.Column<int>(type: "integer", nullable: false),
                    Local = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Link = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ContatoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBCompromisso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBCompromisso_TBContato_ContatoId",
                        column: x => x.ContatoId,
                        principalTable: "TBContato",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBDespesa_TBCategoria",
                columns: table => new
                {
                    CategoriasId = table.Column<Guid>(type: "uuid", nullable: false),
                    DespesasId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBDespesa_TBCategoria", x => new { x.CategoriasId, x.DespesasId });
                    table.ForeignKey(
                        name: "FK_TBDespesa_TBCategoria_TBCategoria_CategoriasId",
                        column: x => x.CategoriasId,
                        principalTable: "TBCategoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBDespesa_TBCategoria_TBDespesa_DespesasId",
                        column: x => x.DespesasId,
                        principalTable: "TBDespesa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBItemTarefa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TarefaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBItemTarefa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBItemTarefa_TBTarefa_TarefaId",
                        column: x => x.TarefaId,
                        principalTable: "TBTarefa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBCompromisso_ContatoId",
                table: "TBCompromisso",
                column: "ContatoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBDespesa_TBCategoria_DespesasId",
                table: "TBDespesa_TBCategoria",
                column: "DespesasId");

            migrationBuilder.CreateIndex(
                name: "IX_TBItemTarefa_TarefaId",
                table: "TBItemTarefa",
                column: "TarefaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBCompromisso");

            migrationBuilder.DropTable(
                name: "TBDespesa_TBCategoria");

            migrationBuilder.DropTable(
                name: "TBItemTarefa");

            migrationBuilder.DropTable(
                name: "TBContato");

            migrationBuilder.DropTable(
                name: "TBCategoria");

            migrationBuilder.DropTable(
                name: "TBDespesa");

            migrationBuilder.DropTable(
                name: "TBTarefa");
        }
    }
}
