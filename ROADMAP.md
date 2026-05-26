# ROADMAP — App de Gastos

Plan completo del proyecto. Actualizar a medida que avanzamos: marcar tareas completadas, añadir decisiones nuevas, registrar cambios de plan.

---

## ✅ Fase 1 — Infraestructura (COMPLETADA)

- [x] Instalar Node.js, Git, .NET 10 SDK, VS Code + C# Dev Kit.
- [x] Crear cuenta GitHub + repo `gastos-app`.
- [x] Crear proyecto Blazor WebAssembly con PWA.
- [x] Crear cuenta Supabase + proyecto.
- [x] Crear esquema de BBDD (6 tablas + RLS + trigger).
- [x] Conectar Blazor con Supabase (`SupabaseService`, `AuthService`).
- [x] Implementar pantalla de Login y protección de Home.
- [x] Configurar GitHub Actions para deploy automático.
- [x] Desplegar en GitHub Pages.
- [x] Verificar instalación como PWA en móvil.

---

## 🟡 Fase 2 — Funcionalidad core

Objetivo: que la app sea utilizable para mi día a día y deje obsoleto el Excel.

### 2.1 — Configuración inicial del usuario ✅ COMPLETADA

Tras el primer login, el usuario no tiene cuentas creadas. Flow para crearlas.

- [x] Detectar si el usuario logueado tiene cuentas (`accounts` vía `account_members`).
- [x] Si no las tiene, redirigir a `/setup`.
- [x] ~~Wizard de 3 pasos~~ → **Pantalla única** con dos campos (más rápido, menos código).
- [x] Al confirmar: crear ambas cuentas y añadir al usuario como miembro (`share_percent` 100 personal / 50 compartida).
- [ ] ~~Crear categorías por defecto~~ → **Pospuesto**: se gestionarán desde su propia pantalla (2.4) o al añadir el primer gasto.

### 2.2 — Gestión de categorías ✅ COMPLETADA

> Reordenado: originalmente era 2.4, pero se necesita antes de poder añadir gastos
> (un gasto requiere `category_id`, y decidimos no crear categorías por defecto en setup).

- [x] RLS policies de `categories` en Supabase (INSERT/SELECT/UPDATE/DELETE).
- [x] Modelo `Category.cs` y servicio `CategoryService.cs` (CRUD).
- [x] Página `/categorias` con pestañas por cuenta (personal / compartida).
- [x] Crear nueva categoría (nombre + bloque). Bloque restringido según tipo de cuenta.
- [x] Editar / eliminar (formulario inline reutilizado).
- [x] Mensaje de error específico si no se puede borrar por gastos asociados (guard preparado, se probará en 2.3).
- [x] Enlace en `NavMenu`.
- [ ] ~~Color / icono~~ → **Pospuesto** a Fase 3 (pulido visual). Columnas a `null` por ahora.

### 2.3 — Añadir gasto rápido ✅ COMPLETADA

La pantalla más importante. Pensada para móvil, una mano, 10 segundos.

- [x] RLS policies de `expenses` en Supabase (con `paid_by = auth.uid()` en INSERT).
- [x] Modelo `Expense.cs` y `ExpenseService.cs` (solo `CreateAsync` por ahora).
- [x] Página `/nuevo-gasto` con pestañas de cuenta y formulario móvil-first.
- [x] Importe (input grande con `inputmode="decimal"` para teclado numérico en móvil).
- [x] Descripción opcional (texto corto).
- [x] Selector de categoría (chips horizontales).
- [x] Fecha (default hoy).
- [x] Botón guardar grande con validación (importe + categoría).
- [x] Feedback visual al guardar (toast verde "✓ Gasto guardado" 2s + reset formulario manteniendo cuenta).
- [x] Enlace en NavMenu y botón CTA grande en Home.

### 2.4 — Vista del mes ✅ COMPLETADA

Para revisar lo gastado y entender dónde se va el dinero.

- [x] `ExpenseService` extendido: `GetByMonthAsync`, `GetByIdAsync`, `UpdateAsync`, `DeleteAsync`.
- [x] Componente reutilizable `ConfirmModal.razor` (patrón `TaskCompletionSource`).
- [x] Página `/mes` con selector de mes (flechas anterior/siguiente).
- [x] Pestañas Personal / Compartida (consistencia con resto de pantallas).
- [x] Total del mes destacado.
- [x] Bloques en acordeón colapsado por defecto, con totales y nº de gastos.
- [x] Cada gasto con descripción/categoría, fecha, importe y acciones (editar/borrar).
- [x] Edición: reutiliza `/nuevo-gasto?id={guid}` (cero duplicación de formulario).
- [x] Borrado con confirmación modal.
- [x] Enlace en NavMenu y botón en Home.

### 2.5 — Previsión mensual y "resto disponible" ✅ COMPLETADA

Equivalente al H15/H16 del Excel.

- [x] RLS policies de `monthly_budgets` en Supabase.
- [x] Modelo `MonthlyBudget.cs` y `BudgetService.cs` con `GetForMonthAsync` y `UpsertAsync`.
- [x] Bloque de previsión integrado en `/mes`: previsión / gastado / resto + barra de progreso semáforo (verde <80%, ámbar 80-99%, rojo ≥100%).
- [x] Modal de edición inline reutilizando estilos de `ConfirmModal`.
- [x] Resto en rojo cuando es negativo.
- [x] Bonus: botón "+ Añadir gasto" en `/mes` que preselecciona la cuenta activa (`/nuevo-gasto?accountId={guid}`).
- [x] **Saldo bancario acumulativo** (2026-05-24): retomado y resuelto con el modelo aclarado por Javi.
    - Migración: `opening_balance` e `income` añadidos a `monthly_budgets` (ver `supabase/migrations.sql`).
    - Personal: saldo_inicial + nómina − aporte_a_compartida (50% del aporte conjunto) − gastos = saldo_final.
    - Compartida: saldo_inicial + aporte_conjunto − gastos = saldo_final.
    - Modal genérico para editar saldo / nómina / aporte (reutiliza el patrón).
    - Auto-arrastre: al editar saldo inicial de un mes sin definir, se precarga la sugerencia calculada del mes anterior. El usuario confirma (no se guarda automático).
    - `share_percent` hardcoded a 50% por ahora (TODO en código para leerlo de `account_members` cuando entre Marta).

### 2.6 — Importar histórico del Excel [POSPUESTO]

> 2026-05-23: Javi decide no importarlo de momento. Se queda como tarea opcional para cuando tenga ganas. El histórico no es crítico — la app ya es usable desde hoy hacia adelante.

- [ ] Script (C# o consola) que lea el Excel actual (Sept 2025 - May 2026).
- [ ] Mapear filas a `expenses` con cuenta, categoría, fecha, importe.
- [ ] Crear categorías que aún no existan.
- [ ] Idempotente (poder ejecutarlo varias veces sin duplicar).

---

## 🟡 Fase 3 — Pulido y mejoras

- [x] Dashboard / resumen con gráficos (`Blazor-ApexCharts` v6).
    - Pantalla `/dashboard` con pestañas Personal / Compartida.
    - KPI: total mes + comparativa vs mes anterior (↑ rojo / ↓ verde).
    - Gráfico de barras: evolución últimos 6 meses.
    - Tarta: reparto por categoría del mes.
    - Una sola query al rango de 6 meses (`GetByDateRangeAsync`) y agregaciones en memoria.
    - Truco contra el "ghost data" al cambiar pestañas: `@key` ligado a `selectedAccountId` + reset de colecciones + `Task.Yield()` antes de recargar.
- [ ] ~~Vista de categorías con tarta de gastos~~ (cubierto por la tarta del dashboard).
- [ ] Filtros y búsqueda en histórico.
- [ ] Edición masiva de gastos.
- [x] Personalizar nombre de app, icono, colores (sustituir branding default de Blazor).
    - App renombrada a "Gastos" en title, manifest, navbar.
    - Paleta grafito + indigo: variables CSS en `app.css` (`--accent`, `--accent-hover`, `--accent-soft`, `--surface-dark`, `--surface-deep`).
    - Sidebar con gradiente vertical (grafito → indigo oscuro).
    - Icono SVG generado (G indigo sobre fondo con el mismo gradiente).
    - Sidebar oculto cuando no estás logueado (MainLayout suscrito a `AuthService.OnAuthStateChanged`).
    - Limpieza: link "About" eliminado, `sample-data/weather.json` y PNGs del template borrados.
- [ ] Modo oscuro.
- [ ] Animaciones/transiciones suaves.

---

## ⚪ Fase 4 — Marta entra

- [ ] Pantalla "invitar a la cuenta compartida".
- [ ] Sistema de invitación: link o código.
- [ ] Marta se registra, acepta, queda añadida a `account_members`.
- [ ] Vista de "balance" entre miembros (quién debe a quién en gastos compartidos).
- [ ] Filtro "pagado por" en la vista del mes.

---

## ⚪ Fase 5 — Extras opcionales

- [ ] Modo offline real (con cola de gastos pendientes de sincronizar).
- [ ] Exportar a Excel / CSV.
- [ ] Recordatorios push (gastos recurrentes, ejemplo: cobro del gimnasio).
- [ ] Importar desde extracto bancario (CSV de la entidad).
- [ ] Categorización automática por patrón de descripción.
- [ ] Multiples monedas (si viajo).
- [ ] Estadísticas anuales.

---

## Decisiones de diseño tomadas

| Tema | Decisión | Razón |
|------|----------|-------|
| Stack | Blazor WASM + Supabase | Alineado con .NET del trabajo, sin escribir backend |
| Hosting | GitHub Pages | Gratis, soporte nativo .NET via Actions |
| Repo | Público | GitHub Pages free requiere público, sin secretos en el código |
| Split común | Tabla `account_members` con `share_percent` | Permite añadir Marta sin migrar nada |
| Categorías | Por cuenta, no globales | Permite que personal y compartida tengan tags distintos |
| Bloques | Campo `block` enum en `categories` | Mismos 4 bloques del Excel + "individual" |
| Routing | Rutas relativas (`NavigateTo("login")`) | Compatible con subdirectorio de GitHub Pages |
| "Cuenta compartida" | = tarjeta bancaria conjunta, no compartición entre usuarios de la app | Yo gestiono ambas; la distinción real es "qué tarjeta pagó" |
| Onboarding | Pantalla única, no wizard de 3 pasos | Más simple y rápido para algo que se hace una sola vez |
| Categorías iniciales | NO se crean en setup | Setup mínimo; se gestionan desde su propia pantalla cuando toque |
| SQL de RLS | Versionado en `supabase/policies.sql` | Reproducible en otros entornos sin re-descubrir el bug del RETURNING |

---

## Cambios y notas posteriores

### 2026-05-24 — Saldo bancario acumulativo: modelo final

Tras la conversación pendiente desde el 23, Javi aclaró el modelo: la previsión de la cuenta compartida representa el **aporte conjunto** al banco, no el presupuesto de gasto. Implementado así:

- **Personal**: saldo_inicial + nómina − aporte_a_compartida (50% del aporte conjunto del mismo mes) − gastos_personales = saldo_final.
- **Compartida**: saldo_inicial + aporte_conjunto − gastos_compartidos = saldo_final.

La previsión personal como "presupuesto de gasto" se eliminó (no encaja en este modelo). Si en el futuro hay que limitar gasto personal se añadirá como campo separado.

Auto-arrastre del saldo: al abrir el modal de saldo inicial en un mes sin definir, se precalcula el saldo final del mes anterior y se ofrece como sugerencia editable. No se guarda hasta que el usuario confirme — permite ajustar contra el banco real.

### 2026-05-23 — Saldo bancario acumulativo pospuesto

Al implementar 2.5 surgió la idea de añadir un saldo acumulado de la cuenta del banco (lo sobrante de un mes pasa al siguiente, como en el Excel). Al detallar el modelo aparecieron tres opciones:

1. **MVP simple**: solo "ingresos del mes" por cuenta-mes, sin acumulado entre meses. Responde a "¿cuánto me sobra este mes?".
2. **Saldo acumulado completo**: ingresos + saldo inicial editable + saldo final que arrastra al siguiente mes.
3. **Posponer**.

Conversación útil: hay dos preguntas conceptualmente distintas que se confunden — "cuánto me sobra de la nómina este mes" (flujo de caja) vs "cuánto hay AHORA mismo en el banco" (saldo acumulado). La segunda requiere registro exhaustivo y suele descuadrar con la realidad del banco por movimientos atípicos.

Decisión: **posponer hasta que esté más claro qué se necesita**. La idea original de Javi era que la nómina alimenta la personal y el aporte a la compartida = previsión × share_percent — modelo correcto pero asume que el aporte real coincide con la previsión, lo cual no siempre es así. Cuando volvamos a esto, partir de la opción simple antes que de la grande.

### 2026-05-23 — Reorden 2.2 ↔ 2.4: categorías antes que gastos

Al planificar la pantalla de añadir gasto rápido (original 2.2) caímos en que un `expense` requiere `category_id`, y en 2.1 decidimos NO crear categorías por defecto en el setup. Sin categorías no se puede crear ningún gasto. Tres opciones discutidas:

1. Crear categorías al vuelo desde la propia pantalla de añadir gasto.
2. Adelantar 2.4 (gestión de categorías) antes que 2.2.
3. Sembrar categorías mínimas en setup (reabrir decisión de 2.1).

Elegida **opción 2**: separación clara de responsabilidades, la pantalla de "añadir gasto" queda enfocada en su único trabajo, y la gestión de categorías ya estaba en el roadmap igualmente. La numeración 2.2/2.3/2.4 se ha reordenado en consecuencia.

### 2026-05-23 — Cierre Fase 2.1 + lección sobre RLS y `RETURNING`

Al implementar el flow de creación de cuentas nos topamos con un `42501 new row violates row-level security policy` aunque la `WITH CHECK` del INSERT estaba bien y el JWT llegaba como `authenticated`. La causa: el cliente Supabase .NET envía `Prefer: return=representation` por defecto, lo que hace que PostgreSQL ejecute internamente `INSERT ... RETURNING *`. En ese caso, **la `USING` de la SELECT policy se aplica como check adicional sobre la fila recién insertada**; si no la pasa, devuelve 42501 con el mismo mensaje que un WITH CHECK fallido. Solución: la SELECT policy de `accounts` debe permitir leer la cuenta también por `owner_id = auth.uid()`, no solo por ser miembro (porque el membership se crea justo después, no antes). Detalle escrito en `supabase/policies.sql`.

Bug paralelo arreglado: las columnas `created_at` / `joined_at` se enviaban con `DateTime.MinValue` desde el cliente y sobrescribían el `DEFAULT now()` de Postgres. Marcadas con `[Column(..., ignoreOnInsert: true, ignoreOnUpdate: true)]` para que el cliente las omita en el body.
