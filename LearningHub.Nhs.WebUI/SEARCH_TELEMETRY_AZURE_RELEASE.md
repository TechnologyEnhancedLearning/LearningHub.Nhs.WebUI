# Search Telemetry Release Notes (Azure AI Search Click Tracking)

## Scope
- WebUI now loads search click telemetry from a dedicated script: `wwwroot/js/search-telemetry.js`.
- Telemetry is only loaded on the Search page (`Views/Search/Index.cshtml`).
- Endpoint used by frontend telemetry: `POST /api/Search/RecordResultClickTelemetry`.

## Telemetry Connection Details
1. User opens Search results page.
2. Search result links include telemetry metadata in `data-*` attributes (correlation/session/query/result info).
3. `search-telemetry.js` listens for:
   - normal click
   - keyboard activation
   - new-tab/middle-click interactions
4. Browser sends non-blocking beacon payload to:
   - `/api/Search/RecordResultClickTelemetry`
5. API writes structured telemetry to Application Insights custom events (`TrackEvent`).
6. Data is queried in Azure Monitor / Log Analytics for dashboards/workbooks.

## Payload Fields Sent from WebUI
- `correlationId`
- `sessionId`
- `queryText`
- `queryMode`
- `resultUrl`
- `resultTitle`
- `resultRank`
- `resourceReferenceId`
- `nodePathId`
- `resultType`
- `openInNewTab`
- `interactionType`

## Azure Setup / Release Checklist
1. Ensure WebUI has Application Insights enabled (already wired in code via `AddApplicationInsightsTelemetry()`).
2. Confirm environment has valid Application Insights connection string/instrumentation configured.
3. Deploy WebUI release.
4. Validate ingestion:
    - run a search
    - click a result
    - verify entries in Application Insights `customEvents`.
5. Create/Update Azure Workbook with KQL against `customEvents`.

## Example KQL (Log Analytics / Application Insights)
```kusto
customEvents
| where name == "SearchResultClickTelemetry"
| extend correlationId = tostring(customDimensions["CorrelationId"])
| extend sessionId = tostring(customDimensions["SessionId"])
| extend queryText = tostring(customDimensions["QueryText"])
| extend resultRank = todouble(customMeasurements["ResultRank"])
| project timestamp, name, correlationId, sessionId, queryText, resultRank
| order by timestamp desc
```

## Post-Deployment Validation
- Confirm no navigation delays when clicking results.
- Confirm no duplicate telemetry records for a single click action.
- Confirm click events appear for:
  - standard click
  - Ctrl/Cmd-click / middle-click
  - keyboard-triggered result open.
