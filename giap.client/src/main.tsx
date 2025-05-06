import {StrictMode, useEffect} from 'react'
import {createRoot} from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {BrowserRouter, Routes, Route, useParams} from 'react-router';
import {useTranslation} from 'react-i18next';

// Wrapper that sets the language based on the URL
// from IBAN issuer: https://github.com/privacybydesign/yivi-iban-issuer/tree/main/react-cra
function LanguageRouter() {
    const {lang} = useParams();
    const {i18n} = useTranslation();

    useEffect(() => {
        if (lang && i18n.language !== lang) {
            i18n.changeLanguage(lang);
        }
    }, [lang, i18n]);

    return (
        <Routes>
            <Route path="/:slug" element={<App/>}/>
        </Routes>
    );
}

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <BrowserRouter>
            <Routes>
                <Route path=":lang/*" element={<LanguageRouter/>}/>
            </Routes>
        </BrowserRouter>
    </StrictMode>,
)
