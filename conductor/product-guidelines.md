# Product Guidelines

## Documentation and Voice
- **Tone**: Professional, clear, and accessible.
- **Objective**: Ensure that documentation and manager tooltips are easy to understand for both developers and content editors.
- **Standard**: Technical accuracy must be balanced with clarity. Avoid unnecessary jargon when describing core CMS functionality.

## Branding and Visual Style
- **Identity**: RavenDB-Inspired.
- **Direction**: Modernize the Piranha CMS interface to reflect a "document-first" aesthetic. The design should feel fast, modern, and aligned with high-performance data systems.

## User Experience (UX)
- **Primary Principle**: Rich Information.
- **Core Standard**: The manager interface should provide deep visibility into the content structure and document states. Information should be layered so that the most critical data is prominent, while detailed technical metadata remains easily accessible.

## Error Handling
- **Strategy**: User-Friendly Feedback.
- **Implementation**: When RavenDB or data-access errors occur, the system should present a clear, non-technical explanation and suggest recovery actions (e.g., "Retrying connection..." or "Please check your document permissions"). Raw database errors should be relegated to background logs.
