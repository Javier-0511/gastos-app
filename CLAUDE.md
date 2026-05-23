# CLAUDE.md

Este archivo da contexto permanente a Claude Code para cualquier sesión sobre este proyecto. Léelo entero al iniciar y consulta `ROADMAP.md` para el plan detallado y la fase actual.

## Sobre el desarrollador

- Soy **Javi**, programador desde cero. Literalmente nunca había tocado código antes de este proyecto.
- Mi trabajo usa **.NET**, por eso elegimos ese stack: aprender algo que veré en el día a día.
- Aprendo programando, no leyendo. Explícame las cosas según las hacemos.

## Reglas de trabajo

1. **Despacio y explicando.** Para cualquier código nuevo, explica qué hace y por qué. No me hagas todo automáticamente.
2. **Plan antes de cambios grandes.** Si vas a crear/refactorizar varios archivos, primero explícame el plan y lo discutimos. Para cambios pequeños, edita directamente.
3. **Recuérdame hacer commit + push** después de cambios significativos.
4. **Si te falta información, pregunta.** No asumas estructuras o datos que no estén en el código.
5. **Mantén actualizado `ROADMAP.md`** cuando completemos hitos o tomemos decisiones nuevas.
6. **No instales paquetes pesados ni añadas dependencias nuevas sin avisarme.** Si lo necesitas, propónlo.

## Objetivo del proyecto

App para controlar mis gastos personales y de cuenta compartida con mi pareja Marta (de momento solo yo uso la app; ella entra en una fase posterior). Reemplaza una hoja Excel con:
- Cuenta personal mía + cuenta compartida (split 50/50).
- 4 bloques en la compartida: fijos, comida, variables, minicompras.
- Bloque individual aparte para gastos personales.
- Categorías/tags (padel, gasolina, cerves, etc.) sumadas por categoría.
- Previsión mensual vs gasto real, con "resto disponible".
- Vista por mes.

## Stack técnico

- **Frontend:** Blazor WebAssembly + C# en .NET 10.
- **Base de datos + auth:** Supabase (PostgreSQL).
- **Hosting:** GitHub Pages con deploy automático vía GitHub Actions.
- **Editor:** VS Code con C# Dev Kit.
- **Repo:** github.com/Javier-0511/gastos-app (público).
- **URL en producción:** https://javier-0511.github.io/gastos-app/

## Estructura del proyecto

- **Solución:** raíz del repo (`C:\Repos`)
- **Proyecto principal:** `GastosApp.Client/` (Blazor WASM)
- **Workflow CI/CD:** `.github/workflows/deploy.yml`
- **Servicios:**
  - `Services/SupabaseService.cs` (cliente Supabase, conexión y init)
  - `Services/AuthService.cs` (login, logout, estado sesión, evento `OnAuthStateChanged`)
- **Páginas:**
  - `Pages/Home.razor` (protegida; redirige a login si no hay sesión)
  - `Pages/Login.razor` (formulario email/contraseña contra Supabase)
- **Credenciales:** `wwwroot/appsettings.json` (Url + AnonKey de Supabase).

## Convenciones de routing

⚠️ La app se sirve desde subdirectorio (`/gastos-app/` en producción). Por eso:
- `NavigateTo` **siempre con rutas relativas** (sin barra inicial): `Nav.NavigateTo("login")`, no `Nav.NavigateTo("/login")`.
- `<NavLink href="...">` igual: sin barra inicial.
- El workflow ya ajusta `<base href="/gastos-app/" />` automáticamente al desplegar.

## Modelo de datos

Tablas en `public` con Row Level Security activa:

- **profiles** (id uuid PK→auth.users, display_name, created_at)
- **accounts** (id, name, is_shared, owner_id→profiles, created_at)
- **account_members** (account_id, user_id, share_percent default 50, joined_at) — PK compuesta
- **categories** (id, account_id, name, block CHECK in [fijo,comida,variable,minicompra,individual], color, icon, created_at) — unique(account_id, name)
- **expenses** (id, account_id, category_id, paid_by→profiles, description, amount, expense_date, created_at)
- **monthly_budgets** (id, account_id, year, month, amount) — unique(account_id, year, month)

Políticas RLS: cada usuario solo ve filas de cuentas donde es miembro. Trigger `handle_new_user` crea perfil automáticamente al registrarse en `auth.users`.

## Estado actual

**Fase 1 (infraestructura): COMPLETADA.**
- Repo en GitHub, proyecto Blazor WASM creado.
- Supabase configurado (tablas, RLS, triggers).
- Login/logout funcionando.
- Deploy automático en GitHub Pages.
- App instalable como PWA.

**Fase 2 (funcionalidad): en curso.** Ver `ROADMAP.md` para detalle y siguiente tarea.

## Cómo arrancar la app en local

```powershell
cd C:\Repos\GastosApp.Client
dotnet run
```

Abre `http://localhost:5143`. Para parar: `Ctrl+C`.

## Flujo de cambios

```powershell
cd C:\Repos
git add .
git commit -m "Mensaje claro en presente"
git push
```

GitHub Actions despliega solo en ~2 min.
