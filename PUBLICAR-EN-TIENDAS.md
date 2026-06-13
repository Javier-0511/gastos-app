# Publicar la app en las tiendas (Play Store / App Store)

Notas para el futuro: opciones, costes y esfuerzo para llevar GastosApp a las
tiendas de móvil. Hoy la app es una **web Blazor WebAssembly** que ya funciona
como **PWA** (instalable desde el navegador).

## Qué tenemos ahora

- App **Blazor WebAssembly**: se descarga al navegador y corre ahí.
- Ya es **PWA**, por lo que se puede instalar **sin pasar por ninguna tienda**:
  - **Android (Chrome):** "Añadir a pantalla de inicio" / "Instalar app".
  - **iPhone (Safari):** Compartir → "Añadir a pantalla de inicio".
- Para uso personal (Javi y Marta) esto **ya es suficiente** y cuesta 0 €.

## Caminos para llegar a las tiendas

### 1. Google Play (Android) — viable y barato ✅ recomendado para empezar

- Técnica: **TWA** (Trusted Web Activity), un envoltorio mínimo que abre la PWA
  a pantalla completa. Google Play lo **acepta oficialmente**.
- Herramienta: **PWABuilder** (https://www.pwabuilder.com/). Le das la URL
  (`https://javier-0511.github.io/gastos-app/`) y te genera el paquete `.aab`
  listo para subir.
- **Coste:** 25 € pago **único** (cuenta de desarrollador de Google).
- **Esfuerzo:** bajo. Funciona desde Windows. Lo más técnico es firmar y subir.

### 2. App Store (iPhone) — viable pero más cuesta arriba

- Necesitas **Mac + Xcode** (no se puede empaquetar para iOS desde Windows).
- **Coste:** 99 €/año (recurrente).
- **Riesgo de rechazo:** Apple a veces rechaza apps que son "solo una web
  envuelta" (regla 4.2, *minimum functionality*).
- PWABuilder también genera el paquete iOS, pero hay más fricción.

### 3. Vía nativa de verdad: .NET MAUI Blazor Hybrid

- Reutilizas los componentes `.razor` dentro de una app nativa real
  (`BlazorWebView` en un caparazón nativo). Genera apps para Android, iOS,
  Windows y Mac.
- **Ventaja:** apps nativas reales + acceso a funciones del móvil
  (notificaciones, biométrico…). Y se aprende MAUI, muy del mundo .NET.
- **Coste:** más trabajo (reestructurar el proyecto) y sigues necesitando
  Mac + cuentas de desarrollador para publicar.

## Plan por fases recomendado

1. **Ahora:** quedarse con la PWA instalable. Cero coste, cero burocracia.
2. **Si quiero "tienda":** empezar solo por **Google Play vía PWABuilder**
   (25 € únicos, Windows vale). Mejor coste/beneficio con diferencia.
3. **App Store / MAUI:** dejarlo para cuando la app esté madura y compense el
   Mac + los 99 €/año.

## Nota importante

La app guarda datos en **Supabase con login**, así que en cualquier formato
(PWA o tienda) el usuario necesita una cuenta. Las tiendas no cambian eso.

## Costes resumidos

| Vía            | Coste                | Plataforma para construir |
|----------------|----------------------|---------------------------|
| PWA (actual)   | 0 €                  | Cualquiera                |
| Google Play    | 25 € único           | Windows vale (PWABuilder) |
| App Store      | 99 €/año             | Mac obligatorio           |
| MAUI Hybrid    | + las cuentas arriba | Windows (Android) / Mac (iOS) |
