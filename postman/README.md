# Postman — Decco.API

`Decco.API.postman_collection.json`: os 13 controllers (List, Get, Insert, Update, Delete — todos POST com o envelope do
Decco.Contracts) e os 3 GETs de micro-frontend.

Importe antes as variáveis globais do workspace: `../../postman/Decco.postman_globals.json`. O guia completo (variáveis,
fluxo Insert → Get → Update → Delete, trava que protege os dados de seed e falhas esperadas) está em `../../postman/README.md`.

Para testar as duas APIs juntas, prefira a collection unificada do workspace, `../../postman/Decco.postman_collection.json`:
uma pasta por controller e as variáveis dentro da própria collection, sem precisar das globais.
