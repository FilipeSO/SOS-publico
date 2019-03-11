# ![sos logo](https://raw.githubusercontent.com/FilipeSO/SOS-publico/master/SOS/Resources/Dangerous%20Creatures%20Recycle%20Full.ico) SOS - Suporte à Operação de Sistemas Elétricos
![AppVeyor](https://img.shields.io/appveyor/ci/FilipeSO/sos-publico.svg)
![GitHub](https://img.shields.io/github/license/FilipeSO/SOS-publico.svg)

Aplicação destinada a gestão, controle de versão, indexação e pesquisa de documentos relacionados à operação do sistema elétrico. Estão inclusos atualmente na relação de documentos disponíveis na aplicação: 
* Manual de Procedimentos da Operação (MPO - Procedimentos de Rede Módulo 10 e submódulos)
* Mensagens de Operação (MOPs)
* Diagramas Unifilares Rede de Operação
* Diagramas Unifilares Rede de Supervisão
* Diagramas Operativos Hidráulicos
A instalação da aplicação é por usuário, entretanto todos os documentos são salvos e indexados para pesquisa em drive local compartilhado por todos os usuários.

## Como usar
Última versão disponível do software para uso está disponível em Releases. A aplicação irá solicitar confirmação de credenciais da  plataforma [CDRE ONS] (https://cdre.ons.org.br) para iniciar o backup local de documentos.

### Pré-requisitos
* .NET Framework 4.6
* Microsoft Visual C++ 2013

## Configuração para compilar código fonte
* Visual Studio 2017
* Plataforma 86x

## Autor

* **Filipe Salles de Oliveira** - Desde 2016 tentando entender como otimizar indexação e pesquisa de documentos para utilização em salas de controle.
Leia sobre como o SOS evoluiu até chegar a este repositório público:
* EDAO XIV (2016)
* SNPTEE XXIV (2017)
* EDAO XV (2018)

## Licença

Este projeto está licenciado sob a licença MIT - consulte o arquivo [LICENSE.md] (LICENSE.md) para obter detalhes

## AVISO LEGAL
O Operador Nacional do Sistema Elétrico (ONS) não tem qualquer participação com este projeto de software livre e autenticação com sua plataforma [CDRE ONS] (https://cdre.ons.org.br) é de caráter autorizativo individual para permitir o download de documentos que seu nível de acesso garante. A autenticação é necessária para download de quaisquer documentos, independente se disponível ao público, como o MPO, uma vez que a aplicação é destinada à usuários do setor elétrico que não raramente são credenciados à plataforma.
